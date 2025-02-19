using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
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
using Components.Bundle;
using Components.Tick;
using Components.Tools.ExtensionMethods;
using Cysharp.Threading.Tasks;
using Database;
using VComponent.Tools.Singletons;
using Sirenix.Utilities;
using Components.Shop.ShopItems;

namespace Components.Grid
{
	/// Responsible to instantiate the grid with tiles, obstacles and decorations. Handle special machines like marchand or extractors. Handle parcels.
	public class GridController : Singleton<GridController>
	{
		[Header("Generation Parameters")]
		[SerializeField] private int _gridXValue = 64;
		[SerializeField] private int _gridYValue = 64;
		[SerializeField] private float _cellSize = 10;
		[SerializeField] private Vector3 _originPosition = new(0, 0);
		[SerializeField] private bool _showDebug;
		[SerializeField] private bool _loadRandomMap;
		[SerializeField] private string _mapToLoadName;

		[Header("Prefabs")]
		[SerializeField] private GameObject _waterPlanePrefab;

		[Header("Holders")]
		[SerializeField] private Transform _groundHolder;
		[SerializeField] private Transform _objectsHolder;
		[SerializeField] private Transform _obstacleHolder;
		[SerializeField] private Transform _decorationHolder;

		[Header("Configuration")]
		[SerializeField] private RunConfiguration _runConfiguration;

		[Header("Sellers Parameters")]
		[SerializeField] private List<Vector2Int> _sellersCoordinates;

		[Header("Grid Parcels")] 
		[SerializeField] private GridParcel _startParcel;
		[SerializeField] private float _parcelAnimationUnlockTime = 2f;
		
		// Grid
		private readonly Dictionary<Vector2Int, TileController> _tiles = new();
		
		//Sellers & Extractor
		private readonly List<MarchandMachineBehaviour> _sellersBehaviours = new();
		private readonly List<ExtractorMachineBehaviour> _extractorBehaviours = new();
		private readonly List<IngredientTemplate> _extractedIngredients = new();

		public Grid Grid { get; private set; }
		public Vector3 OriginPosition => _originPosition;
		public List<IngredientTemplate> ExtractedIngredients => _extractedIngredients;
		
		// ------------------------------------------------------------------------- MONO -------------------------------------------------------------------------------- 
		private void Start()
		{
			Machine.OnRetrieve += RetrieveMachine;
			Machine.OnMove += ClearMachineGridData;
			BundleChoiceGenerator.OnBundleChoiceConfirm += HandleBundleChoiceConfirm;
			UIIngredientShopItemViewController.OnIngredientBuyed += HandleIngredientBuyed;
			ResolutionFactoryState.OnResolutionFactoryStateStarted += HandleResolutionFactoryStarted;
			GridParcelUnlocker.OnParcelUnlocked += HandleParcelUnlocked;
		}

		private void OnDestroy()
		{
			Machine.OnRetrieve += RetrieveMachine;
			Machine.OnMove -= ClearMachineGridData;
			BundleChoiceGenerator.OnBundleChoiceConfirm -= HandleBundleChoiceConfirm;
            UIIngredientShopItemViewController.OnIngredientBuyed -= HandleIngredientBuyed;
            ResolutionFactoryState.OnResolutionFactoryStateStarted -= HandleResolutionFactoryStarted;
			GridParcelUnlocker.OnParcelUnlocked -= HandleParcelUnlocked;
		}
		
		// ------------------------------------------------------------------------- GRID METHODS ------------------------------------------------------------------------
		
		private void GenerateGrid()
		{
			if (Grid != null)
			{
				ClearGrid();
			}

			if (!_loadRandomMap)
			{
				if (GridGenerator.TryLoadMapByName(_mapToLoadName, out var cells))
				{
					GenerateGridFromSerializedCells(cells);
					AddWaterPlane();
				}
				else
				{
					GenerateEmptyGrid();
				}
				
				UnlockParcel(_startParcel);
				
				return;
			}
			
			if (GridGenerator.TryLoadRandomMap(out var serializedCells))
			{
				GenerateGridFromSerializedCells(serializedCells);
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
		}

		private void GenerateEmptyGrid()
		{
			Debug.Log("Generating empty maps");
			Grid = new Grid(_gridXValue, _gridYValue, _cellSize, _originPosition, _showDebug);
			
			// Instantiate ground blocks 
			for (int x = 0; x < Grid.GetWidth(); x++)
			{
				for (int z = 0; z < Grid.GetHeight(); z++)
				{
					Grid.TryGetCellByCoordinates(x, z, out var chosenCell);
					
					// TILES
					var template = ScriptableObjectDatabase.GetTileTemplateByType(TileType.GRASS);
					var gridObjectController = GridObjectController.InstantiateAndAddToGridFromTemplate(template, chosenCell, Grid, _groundHolder);
					
					if (gridObjectController is TileController tileController)
					{
						_tiles.Add(new Vector2Int(x, z), tileController);
						tileController.SetLockedState(true);
					}
				}
			}
		}
		
		private void GenerateGridFromSerializedCells(SerializedCell[] serializedCells)
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

					AddTileFromSerializedCell(serializedCell, chosenCell);

					// OBSTACLES
					if (serializedCell.ObstacleType != ObstacleType.NONE)
					{
						AddObstacleFromSerializedCell(serializedCell, chosenCell);
					}

					// DECORATIONS
					if (serializedCell.DecorationPositions != null && serializedCell.DecorationPositions.Count > 0)
					{
						AddDecorationsFromSerializedCell(serializedCell, chosenCell);
					}
				}
			}
		}

		private void AddTileFromSerializedCell(SerializedCell serializedCell, Cell chosenCell)
		{
			var template = ScriptableObjectDatabase.GetTileTemplateByType(serializedCell.TileType);
			var gridObjectController = GridObjectController.InstantiateAndAddToGridFromTemplate(template, chosenCell, Grid, _groundHolder);
			if (gridObjectController is TileController tileController)
			{
				_tiles.Add(chosenCell.Coordinates, tileController);
				chosenCell.AddTileToCell(tileController);
				tileController.SetLockedState(true);
			}
		}
		
		private void AddObstacleFromSerializedCell(SerializedCell serializedCell, Cell chosenCell)
		{
			// Read obstacle rotation
			// TODO: Add auto getter in serialized class to do this operation
			float[] rotationArray = serializedCell.ObstacleRotation;
			Quaternion obstacleRotation = new Quaternion(rotationArray[0], rotationArray[1], rotationArray[2], rotationArray[3]);

			// Read obstacle scale
			// TODO: Add auto getter in serialized class to do this operation
			float[] scaleArray = serializedCell.ObstacleScale;
			Vector3 obstacleScale = new Vector3(scaleArray[0], scaleArray[1], scaleArray[2]);
						
			var template = ScriptableObjectDatabase.GetObstacleTemplateByType(serializedCell.ObstacleType);
			var gridObjectController = GridObjectController.InstantiateAndAddToGridFromTemplate(template, chosenCell, Grid, _groundHolder, obstacleRotation, obstacleScale);
			if (gridObjectController is ObstacleController obstacleController)
			{
				chosenCell.AddObstacleToCell(obstacleController);
			}
		}
		
		private void AddDecorationsFromSerializedCell(SerializedCell serializedCell, Cell chosenCell)
		{
			for (int i = 0; i < serializedCell.DecorationPositions.Count; i++)
			{
				// Decoration coordinates.
				// TODO: Add auto getter in serialized class to do this operation
				float[] positionArray = serializedCell.DecorationPositions[i];
				Vector3 decorationPosition = new Vector3(positionArray[0], positionArray[1], positionArray[2]);

				// Decoration rotation.
				// TODO: Add auto getter in serialized class to do this operation
				float[] rotationArray = serializedCell.DecorationRotations[i];
				Quaternion decorationRotation = new Quaternion(rotationArray[0], rotationArray[1], rotationArray[2], rotationArray[3]);

				// Decoration local scale.
				// TODO: Add auto getter in serialized class to do this operation
				float[] scaleArray = serializedCell.DecorationScales[i];
				Vector3 decorationScale = new Vector3(scaleArray[0], scaleArray[1], scaleArray[2]);

				var template = ScriptableObjectDatabase.GetDecorationTemplateByType(serializedCell.DecorationTypes[i]);
				var gridObjectController = GridObjectController.InstantiateAndAddToGridFromTemplate(template, chosenCell, Grid, _groundHolder, decorationRotation, decorationScale);
				if (gridObjectController is DecorationController decorationController)
				{
					decorationController.OverrideGridPosition(decorationPosition);
					chosenCell.AddDecorationToCell(decorationController);
				}
			}
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
				var gridObjectController = GridObjectController.InstantiateAndAddToGridFromTemplate(extractorTemplate, chosenCell, Grid, _objectsHolder);

				if (gridObjectController is MachineController machineController)
				{
					// Make sure that the machine are correctly oriented. 
					if (chosenCell.Coordinates.y == 0)
					{
						machineController.RotatePreview(270);
					}
					if (chosenCell.Coordinates.y == Grid.GetHeight() - 1)
					{
						machineController.RotatePreview(90);
					}
				}
				
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
				var gridObjectController = GridObjectController.InstantiateAndAddToGridFromTemplate(destructorTemplate, chosenCell, Grid, _objectsHolder);

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
		
		// ------------------------------------------------------------------------- PARCELS ------------------------------------------------------------------------ 

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

		// -------------------------------------------------------------------------- EVENT HANDLERS --------------------------------------------------------------------------
		
		private void HandleBundleChoiceConfirm(IngredientsBundle bundle, bool isFirstGameChoice)
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

		private void HandleIngredientBuyed(IngredientTemplate ingredient)
		{
            _extractedIngredients.Add(ingredient);
            UpdateIngredientsToExtract(_extractedIngredients);
        }
		
		private void HandleResolutionFactoryStarted(ResolutionFactoryState state)
		{
			UpdateMarchands(state.StateIndex);
		}
		
		private void HandleParcelUnlocked(GridParcel parcel)
		{
			UnlockParcel(parcel);
		}
		
		// -------------------------------------------------------------------------- DEBUG -------------------------------------------------------------------------- 
		
		[Button(ButtonSizes.Medium)]
		private void UpdateDebug()
		{
			Grid.DrawGridDebug();
		}
	}
}