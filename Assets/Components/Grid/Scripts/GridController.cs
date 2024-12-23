using UnityEngine;
using CodeMonkey.Utils;
using System.Collections.Generic;
using Components.Machines;
using Sirenix.OdinInspector;
using System;
using System.Linq;
using Components.Grid.Tile;
using Components.Grid.Obstacle;
using Components.Ingredients;
using Components.Machines.UIView;
using Components.Inventory;
using Components.Machines.Behaviors;
using Components.Recipes;
using Components.Relics;
using Components.Tools.ExtensionMethods;
using Components.Consumable;
using Database;
using Components.Map;
using Components.Bundle;
using Components.Grid.Decorations;
using Components.Grid.Generator;

namespace Components.Grid
{
	public class GridController : MonoBehaviour
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
		[SerializeField] private RelicController _relicControllerPrefab;
		[SerializeField] private ConsumableController _consumableControllerPrefab;
		[SerializeField] private GameObject _waterPlanePrefab;

		[Header("Holders")]
		[SerializeField] private Transform _groundHolder;
		[SerializeField] private Transform _objectsHolder;
		[SerializeField] private Transform _obstacleHolder;
		[SerializeField] private Transform _decorationHolder;

		[Header("Tiles")]
		[SerializeField] private AllTilesController _tileController;

		[Header("Obstacles")]
		[SerializeField] private AllObstaclesController _obstacleController;
		
		[Header("Decorations")]
		[SerializeField] private AllDecorationsController _decorationController;

		[Header("Ingredients")]
		[SerializeField] private int _extractorsOnGridCount = 4;
		[SerializeField] private int _sellersOnGridCount = 4;

		[Header("Configuration")]
		[SerializeField] private RunConfiguration _runConfiguration;

		[Header("Movement Parameters")] 
		[SerializeField] private bool _snapping;

		[Header("Sellers Parameters")]
		[SerializeField] private List<Vector2Int> _sellersCoordinates;

		// Grid 
		private readonly List<MachineController> _instancedObjects = new();
		private readonly List<RelicController> _instancedRelics = new();
		private List<(int, int)> _extractorPotentialCoordinates = new List<(int, int)>();

		// Preview 
		private RelicController _currentRelicPreview;
		private ConsumableController _currentConsumablePreview;

		private int _currentRotation;
		private UnityEngine.Camera _camera;

		//Sellers and Extractor list
		private List<DestructorMachineBehaviour> _sellersBehaviours;
		private List<ExtractorMachineBehaviour> _extractorBehaviours;

		private bool _isFactoryState = true;

		public Grid Grid { get; private set; }
		public Vector3 OriginPosition => _originPosition;
		public static Action OnGridGenerated;

		// ------------------------------------------------------------------------- MONO -------------------------------------------------------------------------------- 

		private void Start()
		{
			_camera = UnityEngine.Camera.main;
			PlanningFactoryState.OnPlanningFactoryStateStarted += HandlePlanningFactoryState;
			ShopState.OnShopStateStarted += HandleShopState;
			MachineContextualUIView.OnSellMachine += SellMachine;
			ConsumableManager.OnChangeSelectedConsumable += UpdateSelection;
			RelicManager.OnChangeSelectedRelic += UpdateSelection;
			MapGenerator.OnMapChoiceConfirm += HandleMapChoiceConfirm;
			UIOptionsController.OnClearGrid += ClearMachines;
		}

		private void OnDestroy()
		{
			PlanningFactoryState.OnPlanningFactoryStateStarted -= HandlePlanningFactoryState;
			ShopState.OnShopStateStarted -= HandleShopState;
			MachineContextualUIView.OnSellMachine -= SellMachine;
			ConsumableManager.OnChangeSelectedConsumable -= UpdateSelection;
			RelicManager.OnChangeSelectedRelic -= UpdateSelection;
			MapGenerator.OnMapChoiceConfirm-= HandleMapChoiceConfirm;
			UIOptionsController.OnClearGrid -= ClearMachines;
		}

		private void Update()
		{
			//Can't interact with anything if we are not in factory state 
			if (_isFactoryState == false)
			{
				return;
			}

			if (_currentRelicPreview != null || _currentConsumablePreview != null)
			{
				MoveSelection();
			}
			if (Input.GetMouseButton(0))
			{
				AddSelectedRelicToGrid();
				AddSelectedConsumableToGrid();
			}
		}

		// ------------------------------------------------------------------------- SELECTION --------------------------------------------------------------------------- 
		private void InstantiateNewRelicPreview()
		{
			_currentRelicPreview = Instantiate(_relicControllerPrefab);
			_currentRelicPreview.InstantiatePreview(RelicManager.Instance.SelectedRelic, _cellSize);
			_currentRelicPreview.RotatePreview(_currentRotation);
		}

		private void UpdateSelection(RelicTemplate relicTemplate)
		{
			DeletePreview();
			_currentRelicPreview = Instantiate(_relicControllerPrefab);
			_currentRelicPreview.InstantiatePreview(relicTemplate, _cellSize);
			_currentRotation = 0;
		}

		private void UpdateSelection(ConsumableTemplate consumableTemplate)
		{
			DeletePreview();
			_currentConsumablePreview = Instantiate(_consumableControllerPrefab);
			_currentConsumablePreview.InstantiatePreview(consumableTemplate, _cellSize);
		}

		private void DestroySelection()
		{
			if (_currentRelicPreview != null)
			{
				Destroy(_currentRelicPreview.gameObject);
			}
			if (_currentConsumablePreview != null)
			{
				Destroy(_currentConsumablePreview.gameObject);
			}
		}

		private void DeletePreview()
		{
			DestroySelection();
			_currentRelicPreview = null;
			_currentConsumablePreview = null;
		}

		private void MoveSelection()
		{
			if (!_currentRelicPreview && !_currentConsumablePreview)
			{
				return;
			}

			if (!UtilsClass.ScreenToWorldPositionIgnoringUI(Input.mousePosition, _camera, out Vector3 worldMousePosition))
			{
				return;
			}

			if (_currentRelicPreview != null)
			{
				_currentRelicPreview.transform.position = worldMousePosition;
			}
			else if (_currentConsumablePreview != null)
			{
				_currentConsumablePreview.transform.position = worldMousePosition;
			}
		}

		// ------------------------------------------------------------------------- INPUT HANDLERS ------------------------------------------------------------------------- 
		private void AddSelectedRelicToGrid()
		{
			if (!_currentRelicPreview)
			{
				return;
			}

			// Try to get the position on the grid. 
			if (!UtilsClass.ScreenToWorldPositionIgnoringUI(Input.mousePosition, _camera, out Vector3 worldMousePosition))
			{
				return;
			}

			// Try getting the cell 
			if (!Grid.TryGetCellByPosition(worldMousePosition, out Cell chosenCell))
			{
				return;
			}

			// One node of the machine overlap a cell that already contain an object. 
			if (chosenCell.ContainsObject)
			{
				return;
			}

			AddRelicToGrid(_currentRelicPreview.Template, chosenCell);
		}

		private void AddRelicToGrid(RelicTemplate template, Cell chosenCell)
		{
			_instancedRelics.Add(_currentRelicPreview);
			_currentRelicPreview.transform.position = Grid.GetWorldPosition(chosenCell.X, chosenCell.Y) + new Vector3(_cellSize / 2, 0, _cellSize / 2);
			_currentRelicPreview.transform.name = $"{template.RelicName}_{_instancedRelics.Count}";
			_currentRelicPreview.transform.parent = _objectsHolder;

			_currentRelicPreview.ConfirmPlacement(chosenCell, 5, _gridXValue, _gridYValue, Grid);
			InstantiateNewRelicPreview();


			//Remove one machine from the inventory 
			GrimoireController.Instance.RemoveRelicFromPlayerInventory(template);
			DeletePreview();

		}

		public void AddMachineToGrid(MachineController machineController, Cell originCell, bool fetchFromInventory)
		{
			_instancedObjects.Add(machineController);

			machineController.transform.position = Grid.GetWorldPosition(originCell.X, originCell.Y) + new Vector3(_cellSize / 2, 0, _cellSize / 2);
			machineController.transform.name = $"{machineController.Machine.Template.Name}_{_instancedObjects.Count}";
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

		private void AddSelectedConsumableToGrid()
		{
			if (!_currentConsumablePreview)
			{
				return;
			}

			// Try to get the position on the grid. 
			if (!UtilsClass.ScreenToWorldPositionIgnoringUI(Input.mousePosition, _camera, out Vector3 worldMousePosition))
			{
				return;
			}

			// Try getting the cell 
			if (!Grid.TryGetCellByPosition(worldMousePosition, out Cell chosenCell))
			{
				return;
			}

			AddConsumableToGrid(_currentConsumablePreview.Template, chosenCell);
		}

		private void AddConsumableToGrid(ConsumableTemplate template, Cell chosenCell)
		{
			_currentConsumablePreview.ConfirmPlacement(chosenCell);

			//Remove one machine from the inventory 
			GrimoireController.Instance.RemoveConsumableFromPlayerInventory(template);
			DeletePreview();
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
		}

		private void ClearGrid()
		{
			foreach (var machineController in _instancedObjects)
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
			_instancedObjects.Clear();
		}

		private void ClearMachines()
		{
			foreach (var machineController in _instancedObjects)
			{
				Destroy(machineController.gameObject);
			}
			
			Grid.ClearNodes();
			_instancedObjects.Clear();
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
			Grid = new Grid(_gridXValue, _gridYValue, _cellSize, _originPosition, _groundHolder, _showDebug);
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
			Grid = new Grid(_gridXValue, _gridYValue, _cellSize, _originPosition, _groundHolder, false, serializedCells);

			// Instantiate ground blocks
			for (int x = 0; x < Grid.GetWidth(); x++)
			{
				for (int z = 0; z < Grid.GetHeight(); z++)
				{
					Grid.TryGetCellByCoordinates(x, z, out var chosenCell);
					
					// TODO: find a cleaner way to do this operation.
					SerializedCell serializedCell = serializedCells.ToList().Find(cell => cell.X == x && cell.Y == z);

					// TILES
					if (serializedCell.TileType != TileType.NONE)
					{
						_tileController.GenerateTileFromType(chosenCell, Grid, _groundHolder, _cellSize, serializedCell.TileType);
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
		
		// ------------------------------------------------------------------------- STATES METHODS ---------------------------------------------------------------------- 
		private void HandleShopState(ShopState obj)
		{
			_isFactoryState = false;
		}

		private void HandlePlanningFactoryState(PlanningFactoryState obj)
		{
			_isFactoryState = true;
			List<IngredientTemplate> baseIngredients = new List<IngredientTemplate>();

			//Get base ingredients extract by extractors
			foreach(var extractor in _extractorBehaviours)
			{
				baseIngredients.Add(extractor.IngredientTemplate);
			}

			//Get the possible ingredient based on current state index and the base ingredients
			var ingredientsFromRecipes = _runConfiguration.GetPossibleIngredients(obj.StateIndex, baseIngredients);
			var randomIngredientsIndexes = ListExtensionsMethods.GetRandomIndexes(ingredientsFromRecipes.Count, _sellersOnGridCount);
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
				behavior.SetSpecialIngredientTemplate(ingredient);
			}
		}

		// ------------------------------------------------------------------------ MACHINE METHODS ---------------------------------------------------------------------- 
		public void SellMachine(Machine machineToSell, int sellPrice)
		{
			//Reset all cell linked to the machine. 
			foreach (var node in machineToSell.Nodes)
			{
				if (!Grid.TryGetCellByCoordinates(node.GridPosition.x, node.GridPosition.y, out Cell linkedCell))
				{
					continue;
				}

				linkedCell.RemoveNodeFromCell();
			}

			_instancedObjects.Remove(machineToSell.Controller);
			Destroy(machineToSell.Controller.gameObject);

			GrimoireController.Instance.AddMachineToPlayerInventory(machineToSell.Template, 1);
			
			// For destroying the class instance, not sure if this a good way.
			machineToSell = null;
		}

		// -------------------------------------------------------------------------- EXTRACTOR -------------------------------------------------------------------------- 
		private void PlaceExtractors(List<IngredientTemplate> ingredientsToInstantiate)
		{			
			_extractorBehaviours = new List<ExtractorMachineBehaviour>();
			_extractorPotentialCoordinates = new List<(int, int)>();
			
			// Instantiate ground blocks 
			for (int x = 0; x < Grid.GetWidth() - 4; x++)
			{
				for (int z = 1; z < Grid.GetHeight(); z++)
				{
					Grid.TryGetCellByCoordinates(x, z, out var chosenCell);

					// Get the zone where the extractors can be placed 
					if ((x == 0 && z <= Grid.GetWidth() / 2) || z == Grid.GetHeight() - 1 || z == 0)
					{
						_extractorPotentialCoordinates.Add(new ValueTuple<int, int>(x, z));
					}
				}
			}

			var randomExtractorCoordinates = ListExtensionsMethods.GetRandomIndexes(_extractorPotentialCoordinates.Count, ingredientsToInstantiate.Count);
			int extractorIndex = 0;
			for (int i = 0; i < _extractorPotentialCoordinates.Count; i++)
			{
				// We want to place an extractor here. 
				if (randomExtractorCoordinates.Contains(i))
				{
					Grid.TryGetCellByCoordinates(_extractorPotentialCoordinates[i].Item1, _extractorPotentialCoordinates[i].Item2, out var chosenCell);

					//Debug.Log($"Going to place on ({chosenCell.X}, {chosenCell.Y}) an extractor with ingredient: {ingredient}"); 

					var extractorTemplate = ScriptableObjectDatabase.GetScriptableObject<MachineTemplate>("Dispenser");

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
						extractorMachineBehaviour.Init(ingredientsToInstantiate[extractorIndex]);
						_extractorBehaviours.Add(extractorMachineBehaviour);
					}
					extractorIndex++;
				}
			}

			// Sort the randomExtractorCoordinates in descending order, ensuring that removes elements from the list starting with the highest index, preserving the validity of the lower indices.
			randomExtractorCoordinates.Sort((a, b) => b.CompareTo(a));

			// Clear the _extractorPotentialCoordinates from selected coordinate
			foreach (var coordinate in randomExtractorCoordinates)
			{
				_extractorPotentialCoordinates.RemoveAt(coordinate);
			}
		}

		private void AddExtractors(List<IngredientTemplate> ingredientsToInstantiate)
		{
			var randomExtractorCoordinates = ListExtensionsMethods.GetRandomIndexes(_extractorPotentialCoordinates.Count, ingredientsToInstantiate.Count);
			int extractorIndex = 0;
			for (int i = 0; i < _extractorPotentialCoordinates.Count; i++)
			{
				// We want to place an extractor here. 
				if (randomExtractorCoordinates.Contains(i))
				{
					Grid.TryGetCellByCoordinates(_extractorPotentialCoordinates[i].Item1, _extractorPotentialCoordinates[i].Item2, out var chosenCell);

					//Debug.Log($"Going to place on ({chosenCell.X}, {chosenCell.Y}) an extractor with ingredient: {ingredient}"); 

					var extractorTemplate = ScriptableObjectDatabase.GetScriptableObject<MachineTemplate>("Dispenser");

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
						extractorMachineBehaviour.Init(ingredientsToInstantiate[extractorIndex]);
						_extractorBehaviours.Add(extractorMachineBehaviour);
					}
					extractorIndex++;
				}
			}
		}

		private void PlaceSellers()
		{
			//List<(int, int)> sellersPotentialCoordinates = new List<(int, int)>();
			var ingredientsFromRecipes = ScriptableObjectDatabase.GetAllScriptableObjectOfType<RecipeTemplate>().Select(template => template.OutIngredient).ToList();
			var randomIngredientsIndexes = ListExtensionsMethods.GetRandomIndexes(ingredientsFromRecipes.Count, _sellersOnGridCount);
			Queue<IngredientTemplate> selectedIngredients = new Queue<IngredientTemplate>();
			_sellersBehaviours = new List<DestructorMachineBehaviour>();

			for (int i = 0; i < ingredientsFromRecipes.Count; i++)
			{
				if (randomIngredientsIndexes.Contains(i))
				{
					selectedIngredients.Enqueue(ingredientsFromRecipes[i]);
				}
			}

			//var randomExtractorCoordinates = ListExtensionsMethods.GetRandomIndexes(_sellersCoordinates.Count, _sellersOnGridCount);
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

				if (chosenCell.Node.Machine.Behavior is DestructorMachineBehaviour destructorMachineBehaviour)
				{
					_sellersBehaviours.Add(destructorMachineBehaviour);
				}
			}
		}

		// -------------------------------------------------------------------------- MAP CHOICES -------------------------------------------------------------------------- 

		private void HandleMapChoiceConfirm(IngredientsBundle bundle, bool isFirstGameChoice)
		{
			if (isFirstGameChoice)
			{
				GenerateGrid();
				PlaceExtractors(bundle.IngredientsTemplatesList);
				PlaceSellers();
			}
			else
			{
				AddExtractors(bundle.IngredientsTemplatesList);
			}
		}
	}
}