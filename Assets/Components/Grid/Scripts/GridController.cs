using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System;
using System.Linq;
using Components.Machines;
using Components.Machines.Behaviors;
using Components.Grid.Tile;
using Components.Grid.Obstacle;
using Components.Grid.Decorations;
using Components.Grid.Generator;
using Components.Grid.Parcel;
using Components.Ingredients;
using Components.Inventory;
using Components.Map;
using Components.Bundle;
using Components.Tick;
using Components.Tools.ExtensionMethods;
using Cysharp.Threading.Tasks;
using Database;
using Sirenix.Utilities;
using VComponent.Tools.Singletons;

namespace Components.Grid
{
	public class GridController : Singleton<GridController>
	{
		[Header("Generation Parameters")]
		[SerializeField] private int _gridXValue = 64;
		[SerializeField] private int _gridYValue = 64;
		[SerializeField] private float _cellSize = 10;
		[SerializeField] private Vector3 _originPosition = new(0, 0);
		[SerializeField] private bool _showDebug;
		[SerializeField] private bool _loadRandomMap;
		[SerializeField] private int _mapToLoadIndex;

		[Header("Prefabs")]
		[SerializeField] private GameObject _groundTile;
		[SerializeField] private MachineController _machineControllerPrefab;
		[SerializeField] private GameObject _waterPlanePrefab;

		[Header("Holders")]
		[SerializeField] private Transform _groundHolder;
		[SerializeField] private Transform _objectsHolder;
		[SerializeField] private Transform _obstacleHolder;
		[SerializeField] private Transform _decorationHolder;

		[Header("Grid objects")]
		[SerializeField] private AllTilesController _tileController;
		[SerializeField] private AllObstaclesController _obstacleController;
		[SerializeField] private AllDecorationsController _decorationController;

		[Header("Configuration")]
		[SerializeField] private RunConfiguration _runConfiguration;

		[Header("Sellers Parameters")]
		[SerializeField] private List<Vector2Int> _sellersCoordinates;

		[Header("Grid Parcels")] 
		[SerializeField] private GridParcel _startParcel;
		[SerializeField] private float _parcelAnimationUnlockTime = 2f;
		
		// Grid
		private readonly List<MachineController> _machines = new();
		private readonly Dictionary<Vector2Int, TileController> _tiles = new();
		
		//Sellers & Extractor
		private readonly List<MarchandMachineBehaviour> _sellersBehaviours = new();
		private readonly List<ExtractorMachineBehaviour> _extractorBehaviours = new();
		private readonly List<IngredientTemplate> _extractedIngredients = new();

		public Grid Grid { get; private set; }
		
		public Vector3 OriginPosition => _originPosition;
		public List<IngredientTemplate> ExtractedIngredients => _extractedIngredients;
		
		public static Action OnGridGenerated;

		// ------------------------------------------------------------------------- MONO -------------------------------------------------------------------------------- 
		private void Start()
		{
			Machine.OnRetrieve += RetrieveMachine;
			Machine.OnMove += ClearMachineGridData;

			MapGenerator.OnMapChoiceConfirm += HandleMapChoiceConfirm;
			UIOptionsController.OnClearGrid += ClearMachines;
			
			PlanningFactoryState.OnPlanningFactoryStateStarted += HandlePlanningFactoryState;

			GridParcelUnlocker.OnParcelUnlocked += HandleParcelUnlocked;
		}

		private void OnDestroy()
		{
			Machine.OnRetrieve += RetrieveMachine;
			Machine.OnMove -= ClearMachineGridData;

			MapGenerator.OnMapChoiceConfirm-= HandleMapChoiceConfirm;
			UIOptionsController.OnClearGrid -= ClearMachines;
			
			PlanningFactoryState.OnPlanningFactoryStateStarted -= HandlePlanningFactoryState;
		}

		// ------------------------------------------------------------------------- ADD OBJECT ------------------------------------------------------------------------- 
		public void AddMachineToGrid(MachineController machineController, Cell originCell, bool fetchFromInventory)
		{
			_machines.Add(machineController);

			machineController.transform.position = Grid.GetWorldPosition(originCell.X, originCell.Y) + new Vector3(_cellSize / 2, 0, _cellSize / 2);
			machineController.transform.name = $"{machineController.Machine.Template.Name}_{_machines.Count}";
			machineController.transform.parent = _objectsHolder;

			// Adding nodes to the cells.
			foreach (var node in machineController.Machine.Nodes)
			{
				var nodeGridPosition = node.SetGridPosition(new Vector2Int(originCell.X, originCell.Y));

				if (Grid.TryGetCellByCoordinates(nodeGridPosition.x, nodeGridPosition.y, out Cell overlapCell))
				{
					overlapCell.AddNodeToCell(node);
					
					// Add potential connected ports 
					foreach (var port in node.Ports)
					{
						switch (port.Side)
						{
							case Side.DOWN:
								TryConnectPort(port, new Vector2Int(nodeGridPosition.x, nodeGridPosition.y - 1));
								break;
							case Side.UP:
								TryConnectPort(port, new Vector2Int(nodeGridPosition.x, nodeGridPosition.y + 1));
								break;
							case Side.RIGHT:
								TryConnectPort(port, new Vector2Int(nodeGridPosition.x + 1, nodeGridPosition.y));
								break;
							case Side.LEFT:
								TryConnectPort(port, new Vector2Int(nodeGridPosition.x - 1, nodeGridPosition.y));
								break;
							case Side.NONE:
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}
					}
				}
			}
			
			machineController.ConfirmPlacement();
			
			if (fetchFromInventory)
			{
				//Remove one machine from the inventory 
				GrimoireController.Instance.DecreaseMachineToPlayerInventory(machineController.Machine.Template, 1);
			}
		}

		// TODO: ALL OBJECT MUST USE THIS METHOD (OBSTACLES, TILES, MACHINES)
		private GridObjectController AddObjectToGridFromTemplate(GridObjectTemplate template, Cell cell, float cellSize)
		{
			var gridObjectInstance = Instantiate(template.GridObject);
			gridObjectInstance.InstantiateOnGrid(template, Grid.GetWorldPosition(cell.X, cell.Y), cellSize, _groundHolder);
			
			return gridObjectInstance;
		}

		private void TryConnectPort(Port port, Vector2Int neighbourPosition)
		{
			if (Grid.TryGetCellByCoordinates(neighbourPosition.x, neighbourPosition.y, out Cell neighbourCell))
			{
				if (neighbourCell.ContainsNode)
				{
					foreach (var potentialPort in neighbourCell.Node.Ports)
					{
						if (potentialPort.Side == port.Side.Opposite())
						{
							port.ConnectTo(potentialPort);
						}
					}
				}
				else
				{
					port.Disconnect();
				}
			}
		}

		public bool TryGetAllPotentialConnection(List<Node> machineNodes, Vector2Int gridPosition, out List<Port> potentialPorts)
		{
			potentialPorts = new List<Port>();
			
			foreach (var node in machineNodes)
			{
				foreach (var port in node.Ports)
				{
					var potentialNeighbourPosition = new Vector2Int();
				
					switch (port.Side)
					{
						case Side.DOWN:
							potentialNeighbourPosition = new Vector2Int(gridPosition.x, gridPosition.y - 1);
							break;
						case Side.UP:
							potentialNeighbourPosition = new Vector2Int(gridPosition.x, gridPosition.y + 1);
							break;
						case Side.RIGHT:
							potentialNeighbourPosition = new Vector2Int(gridPosition.x + 1, gridPosition.y);
							break;
						case Side.LEFT:
							potentialNeighbourPosition = new Vector2Int(gridPosition.x - 1, gridPosition.y);
							break;
						case Side.NONE:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}

					if (TryGetPotentialConnection(port, potentialNeighbourPosition, out Port potentialPort))
					{
						potentialPorts.Add(potentialPort);
					}
				}
			}

			return potentialPorts.Count > 0;
		}
		
		private bool TryGetPotentialConnection(Port port, Vector2Int neighbourPosition, out Port potentialPort)
		{
			if (Grid.TryGetCellByCoordinates(neighbourPosition.x, neighbourPosition.y, out Cell neighbourCell))
			{
				if (neighbourCell.ContainsNode)
				{
					foreach (var neighbourPort in neighbourCell.Node.Ports)
					{
						if (neighbourPort.Side == port.Side.Opposite())
						{
							potentialPort = neighbourPort;
							return true;
						}
					}
				}
			}

			potentialPort = null;
			return false;
		}
		
		// ------------------------------------------------------------------------- GRID METHODS ------------------------------------------------------------------------ 
		[PropertySpace, Button(ButtonSizes.Medium)]
		private void GenerateGrid()
		{
			if (Grid != null)
			{
				ClearGrid();
			}

			if (!_loadRandomMap)
			{
				if (GridGenerator.TryLoadMapAt(_mapToLoadIndex, out var cells))
				{
					GenerateGridFromTemplate(cells);
					AddWaterPlane();
				}
				else
				{
					GenerateEmptyGrid();
				}
				
				return;
			}
			
			if (GridGenerator.TryLoadRandomMap(out var serializedCells))
			{
				GenerateGridFromTemplate(serializedCells);
				AddWaterPlane();
			}
			else
			{
				GenerateEmptyGrid();
			}
			
			UnlockParcel(_startParcel);
		}

		private void ClearGrid()
		{
			foreach (var machineController in _machines)
			{
				Destroy(machineController.gameObject);
			}

			foreach (Transform groundTile in _groundHolder)
			{
				Destroy(groundTile.gameObject);
			}

			foreach (Transform obstacleTile in _obstacleHolder)
			{
				Destroy(obstacleTile.gameObject);
			}

			foreach (Transform objectTile in _objectsHolder)
			{
				Destroy(objectTile.gameObject);
			}

			Grid.ClearNodes();
			Grid.ClearObstacles();
			_machines.Clear();
		}

		private void ClearMachines()
		{
			foreach (var machineController in _machines)
			{
				Destroy(machineController.gameObject);
			}
			
			Grid.ClearNodes();
			_machines.Clear();
		}

		public bool ScanForPotentialConnection(Vector2Int cellPosition, Side sideToScan, Way desiredWay)
		{
			var neighbourPosition = sideToScan.GetNeighbourPosition(cellPosition);
			
			if (Grid.TryGetCellByCoordinates(neighbourPosition.x, neighbourPosition.y, out var cell))
			{
				if (cell.ContainsNode)
				{
					foreach (var port in cell.Node.Ports)
					{
						if (port.Side == sideToScan.Opposite() && port.Way == desiredWay)
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		private void GenerateEmptyGrid()
		{
			Grid = new Grid(_gridXValue, _gridYValue, _cellSize, _originPosition, _showDebug);
			_tileController.SelectATileType();
			
			// Instantiate ground blocks 
			for (int x = 0; x < Grid.GetWidth(); x++)
			{
				for (int z = 0; z < Grid.GetHeight(); z++)
				{
					Grid.TryGetCellByCoordinates(x, z, out var chosenCell);
					_tileController.GenerateTile(chosenCell, Grid, _groundHolder, _cellSize);
				}
			}
			
			OnGridGenerated?.Invoke();
		}
		
		private void GenerateGridFromTemplate(SerializedCell[] serializedCells)
		{
			Grid = new Grid(_gridXValue, _gridYValue, _cellSize, _originPosition, _showDebug, serializedCells);

			// Instantiate ground blocks
			for (int x = 0; x < Grid.GetWidth(); x++)
			{
				for (int z = 0; z < Grid.GetHeight(); z++)
				{
					Grid.TryGetCellByCoordinates(x, z, out var chosenCell);
					
					// TODO: find a cleaner way to do this operation.
					SerializedCell serializedCell = serializedCells.ToList().Find(cell => cell.X == x && cell.Y == z);

					// TILES
					var template = ScriptableObjectDatabase.GetTileTemplateByType(serializedCell.TileType);
					var gridController = AddObjectToGridFromTemplate(template, chosenCell, _cellSize);
					if (gridController is TileController tileController)
					{
						_tiles.Add(new Vector2Int(x, z), tileController);
						tileController.SetLockedState(true);
					}

					// OBSTACLES
					if (serializedCell.ObstacleType != ObstacleType.NONE)
					{
						// Read obstacle rotation
						float[] rotationArray = serializedCell.ObstacleRotation;
						Quaternion obstacleRotation = new Quaternion(rotationArray[0], rotationArray[1], rotationArray[2], rotationArray[3]);

						// Read obstacle scale
						float[] scaleArray = serializedCell.ObstacleScale;
						Vector3 obstacleScale = new Vector3(scaleArray[0], scaleArray[1], scaleArray[2]);

						// Generate obstacle
						_obstacleController.GenerateObstacleFromType(chosenCell, Grid, _obstacleHolder, _cellSize, serializedCell.ObstacleType, obstacleRotation, obstacleScale);
					}

					// DECORATIONS
					if (serializedCell.DecorationPositions != null && serializedCell.DecorationPositions.Count > 0)
					{
						for (int i = 0; i < serializedCell.DecorationPositions.Count; i++)
						{
							// Decoration coordinates.
							float[] positionArray = serializedCell.DecorationPositions[i];
							Vector3 decorationPosition = new Vector3(positionArray[0], positionArray[1], positionArray[2]);

							// Decoration rotation.
							float[] rotationArray = serializedCell.DecorationRotations[i];
							Quaternion decorationRotation = new Quaternion(rotationArray[0], rotationArray[1], rotationArray[2], rotationArray[3]);

							// Decoration local scale.
							float[] scaleArray = serializedCell.DecorationScales[i];
							Vector3 decorationScale = new Vector3(scaleArray[0], scaleArray[1], scaleArray[2]);

							// Generate decoration.
							_decorationController.GenerateDecorationFromType(chosenCell, _decorationHolder, serializedCell.DecorationTypes[i], decorationPosition, decorationRotation, decorationScale);
						}
					}
				}
			}
			
			OnGridGenerated?.Invoke();
		}
		
		private void AddWaterPlane()
		{
			if (_waterPlanePrefab)
			{
				var waterPlane = Instantiate(_waterPlanePrefab, transform);
				waterPlane.transform.position = OriginPosition + new Vector3(Grid.GetWidth() / 2f, 0, Grid.GetHeight() / 2f);
				waterPlane.transform.localScale = new Vector3(Grid.GetWidth() / 10f, 1, Grid.GetHeight() / 10f);
			}
			else
			{
				Debug.LogError("No water prefab found.");
			}
		}

		private async void UnlockParcel(GridParcel parcel)
		{
			var parcelCoordinates = parcel.Coordinates();
			var tilesToUnlock = new List<TileController>();
			
			// Set the data
			for (int i = 0; i < parcelCoordinates.Count; i++)
			{
				if (Grid.TryGetCellByCoordinates(parcelCoordinates[i], out var cell))
				{
					cell.Unlock();
					tilesToUnlock.Add(_tiles[parcelCoordinates[i]]);
				}
				else
				{
					Debug.LogError($"No cell to unlock fond on {parcelCoordinates[i]}");
				}
			}

			// Animate unlock in parallel (to avoid frame rate issues)
			var tasks = new List<UniTask>();
			for (int i = 0; i < tilesToUnlock.Count; i++)
			{
				float delay = i * (_parcelAnimationUnlockTime / tilesToUnlock.Count);
				tasks.Add(AnimateUnlock(tilesToUnlock[i], delay));
			}
			
			await UniTask.WhenAll(tasks);
		}
		
		private async UniTask AnimateUnlock(TileController tile, float delay)
		{
			await UniTask.WaitForSeconds(delay);
			tile.SetLockedState(false);
		}

		[Button(ButtonSizes.Medium)]
		private void UpdateDebug()
		{
			Grid.DrawGridDebug();
		}
		
		// ------------------------------------------------------------------------ MACHINE METHODS ---------------------------------------------------------------------- 
		private void ClearMachineGridData(Machine machineToClear)
		{
			//Reset all cell linked to the machine. 
			foreach (var node in machineToClear.Nodes)
			{
				if (!Grid.TryGetCellByCoordinates(node.GridPosition.x, node.GridPosition.y, out Cell linkedCell))
				{
					continue;
				}

				linkedCell.RemoveNodeFromCell();
			}
			
			// Potential remove from tickables
			TickSystem.RemoveTickable(machineToClear);
		}

		private void RetrieveMachine(Machine machineToSell, bool giveBack)
		{
			ClearMachineGridData(machineToSell);
			
			// Remove 3D objects
			_machines.Remove(machineToSell.Controller);
			Destroy(machineToSell.Controller.gameObject);

			// Give back to the player
			if (giveBack)
			{
				GrimoireController.Instance.AddMachineToPlayerInventory(machineToSell.Template, 1);
			}
			
			// For destroying the class instance, not sure if this a good way.
			machineToSell = null;
		}

		// -------------------------------------------------------------------------- EXTRACTOR & MARCHANDS -------------------------------------------------------------------------- 
		private List<Vector2Int> GetExtractorRandomCoordinates(int extractorCount)
		{
			var extractorPotentialCoordinates = new List<Vector2Int>();
			var startCoordinates = _startParcel.Coordinates();

			foreach (var startCoordinate in startCoordinates)
			{
				if (Grid.TryGetCellByCoordinates(startCoordinate, out var cell))
				{
					if (!cell.IsConstructable())
					{
						continue;
					}
					
					extractorPotentialCoordinates.Add(startCoordinate);
				}
			}

			// Selecting random coordinates
			var randomExtractorCoordinates = ListExtensionsMethods.GetRandomIndexes(extractorPotentialCoordinates.Count, extractorCount);
			var randomSelectedCoordinates = new List<Vector2Int>();
			for (int i = 0; i < randomExtractorCoordinates.Count; i++)
			{
				randomSelectedCoordinates.Add(extractorPotentialCoordinates[randomExtractorCoordinates[i]]);
			}

			return randomSelectedCoordinates;
		}
 		
		private void AddExtractors(int count)
		{			
			var randomCoordinates = GetExtractorRandomCoordinates(count);
			
			// Placing extractors
			for (int i = 0; i < randomCoordinates.Count; i++)
			{
				Grid.TryGetCellByCoordinates(randomCoordinates[i].x, randomCoordinates[i].y, out var chosenCell);
					
				var extractorTemplate = ScriptableObjectDatabase.GetScriptableObject<MachineTemplate>("Extractor");

				var machine = Instantiate(_machineControllerPrefab);
				machine.InstantiatePreview(extractorTemplate, _cellSize);

				// Make sure that the machine are correctly oriented. 
				if (chosenCell.Y == 0)
				{
					machine.RotatePreview(270);
				}
				if (chosenCell.Y == Grid.GetHeight() - 1)
				{
					machine.RotatePreview(90);
				}

				AddMachineToGrid(machine, chosenCell, false);

				if (chosenCell.Node.Machine.Behavior is ExtractorMachineBehaviour extractorMachineBehaviour)
				{
					_extractorBehaviours.Add(extractorMachineBehaviour);
				}
				else
				{
					Debug.LogError($"An extractor has been placed but no {nameof(ExtractorMachineBehaviour)} found on it.");
				}
			}
		}

		private void UpdateIngredientsToExtract(List<IngredientTemplate> ingredientsToExtract)
		{
			if (ingredientsToExtract.Count != _extractorBehaviours.Count)
			{
				var extractorCountToAdd = ingredientsToExtract.Count - _extractorBehaviours.Count;
				Debug.Log($"You want to extract {ingredientsToExtract.Count} ingredients but there is only {_extractorBehaviours.Count} extractors, adding {extractorCountToAdd} extractors.");
				
				AddExtractors(extractorCountToAdd);
			}
			
			for (int i = 0; i < ingredientsToExtract.Count; i++)
			{
				if (i >= _extractorBehaviours.Count)
				{
					break;
				}
				
				_extractorBehaviours[i].SetExtractedIngredient(ingredientsToExtract[i]);
			}
		}

		private void PlaceMarchands()
		{
			_sellersBehaviours.Clear();

			for (int i = 0; i < _sellersCoordinates.Count; i++)
			{
				if (!Grid.TryGetCellByCoordinates(_sellersCoordinates[i].x, _sellersCoordinates[i].y, out var chosenCell))
				{
					Debug.LogError($"Unable to place seller at ({_sellersCoordinates[i].x}, {_sellersCoordinates[i].y}), there is no cell at this position");
					continue;
				}
				
				var destructorTemplate = ScriptableObjectDatabase.GetScriptableObject<MachineTemplate>("Marchand");

				var machine = Instantiate(_machineControllerPrefab);
				machine.InstantiatePreview(destructorTemplate, _cellSize);
				
				AddMachineToGrid(machine, chosenCell, false);

				if (chosenCell.Node.Machine.Behavior is MarchandMachineBehaviour destructorMachineBehaviour)
				{
					_sellersBehaviours.Add(destructorMachineBehaviour);
				}
			}
		}

		private void UpdateMarchands(int stateIndex)
		{
			List<IngredientTemplate> baseIngredients = new List<IngredientTemplate>();

			//Get base ingredients extract by extractors
			foreach(var extractor in _extractorBehaviours)
			{
				baseIngredients.Add(extractor.IngredientToExtract);
			}

			//Get the ingredient based on current state index and the base ingredients
			var ingredientsFromRecipes = _runConfiguration.GetPossibleIngredients(stateIndex, baseIngredients);
			var randomIngredientsIndexes = ListExtensionsMethods.GetRandomIndexes(ingredientsFromRecipes.Count, _sellersBehaviours.Count);
			Queue<IngredientTemplate> selectedIngredients = new Queue<IngredientTemplate>();

			for (int i = 0; i < ingredientsFromRecipes.Count; i++)
			{
				if (randomIngredientsIndexes.Contains(i))
				{
					selectedIngredients.Enqueue(ingredientsFromRecipes[i]);
				}
			}

			foreach (var behavior in _sellersBehaviours)
			{
				//Add ingredient as special ingredient 
				var ingredient = selectedIngredients.Dequeue();
				behavior.SetFavoriteIngredient(ingredient);
			}
		}

		// -------------------------------------------------------------------------- EVENT HANDLERS -------------------------------------------------------------------------- 
		private void HandleMapChoiceConfirm(IngredientsBundle bundle, bool isFirstGameChoice)
		{
			if (isFirstGameChoice)
			{
				GenerateGrid();
				
				// Marchands
				PlaceMarchands();
			}

			_extractedIngredients.AddRange(bundle.IngredientsTemplatesList);
			UpdateIngredientsToExtract(_extractedIngredients);
		}
		
		private void HandlePlanningFactoryState(PlanningFactoryState state)
		{
			UpdateMarchands(state.StateIndex);
		}
		
		private void HandleParcelUnlocked(GridParcel parcel)
		{
			UnlockParcel(parcel);
		}
	}
}