using UnityEngine;
using CodeMonkey.Utils;
using System.Collections.Generic;
using Components.Machines;
using Sirenix.OdinInspector;
using System;
using System.Linq;
using Components.Economy;
using Components.Grid.Tile;
using Components.Grid.Obstacle;
using Components.Ingredients;
using Components.Machines.UIView;
using Components.Inventory;
using Components.Machines.Behaviors;
using Components.Recipes;
using Components.Relics;
using Components.Tools.ExtensionMethods;
using Database;

namespace Components.Grid
{
    public class GridController : MonoBehaviour
    {
        [Header("Generation Parameters")]
        [SerializeField] private int _gridXValue = 64;
        [SerializeField] private int _gridYValue = 64;
        [SerializeField] private float _cellSize = 10;
        [SerializeField] private Vector3 _startPosition = new(0, 0);
        [SerializeField] private bool _showDebug;
        
        [Header("Prefabs")] 
        [SerializeField] private GameObject _groundTile;
        [SerializeField] private MachineController _machineControllerPrefab;
        [SerializeField] private RelicController _relicControllerPrefab;
        
        [Header("Holders")]
        [SerializeField] private Transform _groundHolder;
        [SerializeField] private Transform _objectsHolder;
        [SerializeField] private Transform _obstacleHolder;

		[Header("Tiles")]
		[SerializeField] private AllTilesController _tileController;

		[Header("Obstacles")]
		[SerializeField] private AllObstaclesController _obstacleController;

		[Header("Ingredients")]
		[SerializeField] private int _extractorsOnGridCount = 4;
		
		// Grid
		private Grid _grid;
		private readonly List<MachineController> _instancedObjects = new ();
		private readonly List<RelicController> _instancedRelics = new ();
		
        // Preview
        private MachineController _currentMachinePreview;
        private RelicController _currentRelicPreview;
        private int _currentRotation;
        private UnityEngine.Camera _camera;
        
        // Actions
        public Action<Machine> OnMachineAdded;
        public Action<Machine> OnMachineRemoved;

        private bool _isFactoryState = true;
        // ------------------------------------------------------------------------- MONO -------------------------------------------------------------------------
        private void Start()
        {
            _camera = UnityEngine.Camera.main;
            MachineManager.OnChangeSelectedMachine += UpdateSelection;
            RelicManager.OnChangeSelectedRelic += UpdateSelection;
            GenerateGrid();

            PlanningFactoryState.OnPlanningFactoryStateStarted += HandlePlanningFactoryState;
            ShopState.OnShopStateStarted += HandleShopState;
            MachineContextualUIView.OnSellMachine += HandleMachineSold;
        }

		private void OnDestroy()
		{
            PlanningFactoryState.OnPlanningFactoryStateStarted -= HandlePlanningFactoryState;
            ShopState.OnShopStateStarted -= HandleShopState;
            MachineContextualUIView.OnSellMachine -= HandleMachineSold;

            RelicManager.OnChangeSelectedRelic -= UpdateSelection;
        }

		private void Update()
        {
            //Can't interact with anything if we are not in factory state
            if(_isFactoryState == false)
			{
                return;
			}

            if(_currentMachinePreview != null || _currentRelicPreview != null)
            {
				MoveSelection();
			}
            
            if (Input.GetMouseButton(1))
            {
                RemoveMachineFromGrid();
            }
            if (Input.GetMouseButton(0))
            {
                AddSelectedMachineToGrid();
                AddSelectedRelicToGrid();
            }
            if (Input.GetMouseButtonDown(2))
            {
                RotateSelection();
            }

        }
        
        // ------------------------------------------------------------------------- SELECTION -------------------------------------------------------------------------
        private void InstantiateNewPreview()
        {
	        if (MachineManager.Instance.SelectedMachine == null)
	        {
		        _currentMachinePreview = null;
		        return;
	        }
	        
			_currentMachinePreview = Instantiate(_machineControllerPrefab);
			_currentMachinePreview.InstantiatePreview(MachineManager.Instance.SelectedMachine, _cellSize);
			_currentMachinePreview.RotatePreview(_currentRotation);
		}

		private void InstantiateNewRelicPreview()
		{
			_currentRelicPreview = Instantiate(_relicControllerPrefab);
			_currentRelicPreview.InstantiatePreview(RelicManager.Instance.SelectedRelic, _cellSize);
			_currentRelicPreview.RotatePreview(_currentRotation);
		}

		private void UpdateSelection(MachineTemplate newTemplate)
        {
            DeletePreview();
            _currentMachinePreview = Instantiate(_machineControllerPrefab);
            _currentMachinePreview.InstantiatePreview(newTemplate, _cellSize);
            _currentRotation = 0;
        }

        private void UpdateSelection(RelicTemplate relicTemplate)
        {
            DeletePreview();
            _currentRelicPreview = Instantiate(_relicControllerPrefab);
            _currentRelicPreview.InstantiatePreview(relicTemplate, _cellSize);
            _currentRotation = 0;
		}
       
        private void DestroySelection()
        {
			if (_currentMachinePreview != null)
			{
				Destroy(_currentMachinePreview.gameObject);
			}
			if (_currentRelicPreview != null)
			{
				Destroy(_currentRelicPreview.gameObject);
			}

		}

        private void DeletePreview()
        {
            DestroySelection();
           _currentMachinePreview = null;
           _currentRelicPreview = null;
        }
        
        private void MoveSelection()
        {
	        if (!_currentMachinePreview && !_currentRelicPreview)
	        {
				return;
	        }

	        if (!UtilsClass.ScreenToWorldPositionIgnoringUI(Input.mousePosition, _camera, out Vector3 worldMousePosition))
            {
                return;
            }

            // Update the object's position
            if(_currentMachinePreview != null)
			{
                _currentMachinePreview.transform.position = worldMousePosition;
			}
            else if (_currentRelicPreview != null)
			{
                _currentRelicPreview.transform.position = worldMousePosition;
			}
        }
        
        private void RotateSelection()
        {
	        if (!_currentMachinePreview && !_currentRelicPreview)
	        {
		        return;
	        }
	        
            _currentRotation += 90;
            _currentRotation %= 360;

            if(_currentMachinePreview != null)
			{
                _currentMachinePreview.RotatePreview(_currentRotation); 
			}
            else if (_currentRelicPreview != null)
			{
                _currentMachinePreview.RotatePreview(_currentRotation);
            }
        }

        // ------------------------------------------------------------------------- INPUT HANDLERS -------------------------------------------------------------------------
        private void AddSelectedMachineToGrid()
        {
	        if (!_currentMachinePreview)
	        {
		        return;
	        }
	        
			// Try to get the position on the grid.
			if (!UtilsClass.ScreenToWorldPositionIgnoringUI(Input.mousePosition, _camera, out Vector3 worldMousePosition))
			{
				return;
			}

			// Try getting the cell
			if (!_grid.TryGetCellByPosition(worldMousePosition, out Cell chosenCell))
			{
				return;
			}
			
			// Check if the machine can be placed on the grid.
			foreach (var node in _currentMachinePreview.Machine.Nodes)
			{
				var nodeGridPosition = node.SetGridPosition(new Vector2Int(chosenCell.X, chosenCell.Y));

				// One node does not overlap a constructable cell.
				if (!_grid.TryGetCellByCoordinates(nodeGridPosition.x, nodeGridPosition.y, out Cell overlapCell))
				{
					return;
				}
					
				// One node of the machine overlap a cell that already contain an object.
				if (overlapCell.ContainsObject)
				{
					return;
				}
			}
			
			AddMachineToGrid(MachineManager.Instance.SelectedMachine, chosenCell);
        }

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
            if (!_grid.TryGetCellByPosition(worldMousePosition, out Cell chosenCell))
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
            _currentRelicPreview.transform.position = _grid.GetWorldPosition(chosenCell.X, chosenCell.Y) + new Vector3(_cellSize / 2, 0, _cellSize / 2);
            _currentRelicPreview.transform.name = $"{template.RelicName}_{_instancedRelics.Count}";
            _currentRelicPreview.transform.parent = _objectsHolder;

            _currentRelicPreview.ConfirmPlacement();
            _currentRelicPreview.DrawZoneGizmos(chosenCell, 5, _gridXValue, _gridYValue, _grid);
            InstantiateNewRelicPreview();


            //Remove one machine from the inventory
            InventoryController.Instance.RemoveRelicFromPlayerInventory(template);
            DeletePreview();

        }
        private void AddMachineToGrid(MachineTemplate machine, Cell originCell)
        {
            _instancedObjects.Add(_currentMachinePreview);

            _currentMachinePreview.transform.position = _grid.GetWorldPosition(originCell.X, originCell.Y) + new Vector3(_cellSize / 2, 0, _cellSize / 2);
            _currentMachinePreview.transform.name = $"{machine.Name}_{_instancedObjects.Count}";
            _currentMachinePreview.transform.parent = _objectsHolder;

            // Adding nodes to the cells
            foreach (var node in _currentMachinePreview.Machine.Nodes)
            {
                var nodeGridPosition = node.SetGridPosition(new Vector2Int(originCell.X, originCell.Y));

                if (_grid.TryGetCellByCoordinates(nodeGridPosition.x, nodeGridPosition.y, out Cell overlapCell))
                {
                    overlapCell.AddNodeToCell(node);

                    // Add potential connected ports
                    foreach (var port in node.Ports)
                    {
                        switch (port.Side)
                        {
                            case Side.DOWN:
                                TryBindConnectedPort(port, new Vector2Int(nodeGridPosition.x, nodeGridPosition.y - 1));
                                break;
                            case Side.UP:
                                TryBindConnectedPort(port, new Vector2Int(nodeGridPosition.x, nodeGridPosition.y + 1));
                                break;
                            case Side.RIGHT:
                                TryBindConnectedPort(port, new Vector2Int(nodeGridPosition.x + 1, nodeGridPosition.y));
                                break;
                            case Side.LEFT:
                                TryBindConnectedPort(port, new Vector2Int(nodeGridPosition.x - 1, nodeGridPosition.y));
                                break;
                            case Side.NONE:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
            //originCell.
            _currentMachinePreview.ConfirmPlacement();

            InstantiateNewPreview();

            //Remove one machine from the inventory
            InventoryController.Instance.DecreaseMachineToPlayerInventory(machine, 1);
            //Check if we don"t have any left of this machine in player inventory 
            if (InventoryController.Instance.PlayerMachinesDictionary[machine] == 0)
            {
                DeletePreview();
            }
        }

        private void TryBindConnectedPort(Port port, Vector2Int neighbourPosition)
        {
	        if (_grid.TryGetCellByCoordinates(neighbourPosition.x , neighbourPosition.y, out Cell neighbourCell))
	        {
		        if (neighbourCell.ContainsNode)
		        {
			        foreach (var potentialPort in neighbourCell.Node.Ports)
			        {
				        if (potentialPort.Side == port.Side.Opposite())
				        {
					        port.SetConnectedPort(potentialPort);
				        }
			        }
		        }
	        }
        }
        
        private void RemoveMachineFromGrid()
        {
            if(_currentMachinePreview)
            {
				DeletePreview();
                return;
			}
        }
        
        // ------------------------------------------------------------------------- GRID METHODS -------------------------------------------------------------------------
        [PropertySpace ,Button(ButtonSizes.Medium)]
        private void GenerateGrid()
        {
            if (_grid != null)
            {
                ClearGrid();
            }
            
            _grid = new Grid(_gridXValue, _gridYValue, _cellSize, _startPosition, _groundHolder, _showDebug);
            _tileController.SelectATileType();

            var ingredientsFromRecipes = ScriptableObjectDatabase.GetAllScriptableObjectOfType<RecipeTemplate>().Select(template => template.OutIngredient);
            var allIngredients = ScriptableObjectDatabase.GetAllScriptableObjectOfType<IngredientTemplate>();

            var baseIngredient = allIngredients.Except(ingredientsFromRecipes).ToList();
            var randomIngredientsIndexes = ListExtensionsMethods.GetRandomIndexes(baseIngredient.Count, _extractorsOnGridCount);

            Queue<IngredientTemplate> selectedIngredients = new Queue<IngredientTemplate>();
            
            for (int i = 0; i < baseIngredient.Count; i++)
            {
	            if (randomIngredientsIndexes.Contains(i))
	            {
		            selectedIngredients.Enqueue(baseIngredient[i]);
	            }
            }
            
            List<(int, int)> extractorPotentialCoordinates = new List<(int, int)>();
            
			// Instantiate ground blocks
			for (int x = 0; x < _grid.GetWidth(); x++)
            {
                for (int z = 0; z < _grid.GetHeight(); z++)
                {
					_grid.TryGetCellByCoordinates(x, z, out var chosenCell);
					
					TileController tile = _tileController.GenerateTile(chosenCell, _grid, _groundHolder, _cellSize);
					
					// Get the zone where the extractors can be placed
					if ((x == 0 && z <= _grid.GetWidth() / 2) || (x == _grid.GetWidth() - 1 && z <= _grid.GetWidth() / 2) || z == 0)
					{
						extractorPotentialCoordinates.Add(new(x, z));
						continue;
					}
					
					if (tile.TileType == TileType.WATER)
					{
						//No need to place anything else on this cell because it is water
						continue;
					}

					if (x != 1 && x != _grid.GetWidth() - 2 && z != 1 && z != _grid.GetHeight() - 2)
					{
						_obstacleController.GenerateObstacle(_grid, chosenCell, _obstacleHolder, _cellSize);
					}
                }
            }

            var randomExtractorCoordinates = ListExtensionsMethods.GetRandomIndexes(extractorPotentialCoordinates.Count, _extractorsOnGridCount);
            for (int i = 0; i < extractorPotentialCoordinates.Count; i++)
            {
	            // We want to place an extractor here.
	            if (randomExtractorCoordinates.Contains(i))
	            {
		            _grid.TryGetCellByCoordinates(extractorPotentialCoordinates[i].Item1, extractorPotentialCoordinates[i].Item2, out var chosenCell);
		            var ingredient = selectedIngredients.Dequeue();
		            
		            Debug.Log($"Going to place on ({chosenCell.X}, {chosenCell.Y}) an extractor with ingredient: {ingredient}");

		            var extractorTemplate = ScriptableObjectDatabase.GetScriptableObject<MachineTemplate>("Extractor");
		            
		            _currentMachinePreview = Instantiate(_machineControllerPrefab);
		            _currentMachinePreview.InstantiatePreview(extractorTemplate, _cellSize);

		            // Make sure that the machine are correctly oriented.
		            if (chosenCell.Y == 0)
		            {
			            _currentMachinePreview.RotatePreview(270);
		            }
		            if (chosenCell.X == _grid.GetWidth() - 1)
		            {
			            _currentMachinePreview.RotatePreview(180);
		            }
		            
		            AddMachineToGrid(extractorTemplate, chosenCell);

		            if (chosenCell.Node.Machine.Behavior is ExtractorMachineBehaviour extractorMachineBehaviour)
		            {
			            extractorMachineBehaviour.Init(ingredient);
		            }
	            }
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

			_grid.ClearCellsData();
            _instancedObjects.Clear();
        }

        // ------------------------------------------------------------------------- STATES METHODS -------------------------------------------------------------------------
        private void HandleShopState(ShopState obj)
        {
            _isFactoryState = false;
        }

        private void HandlePlanningFactoryState(PlanningFactoryState obj)
        {
            _isFactoryState = true;
        }
        
        // --------------------------------------------------------------------- MACHINE METHODS -----------------------------------------------------------
        private void HandleMachineSold(Machine machineToSell, int sellPrice)
        {
	        //Reset all cell linked to the machine.
	        foreach (var node in machineToSell.Nodes)
	        {
		        if (!_grid.TryGetCellByCoordinates(node.GridPosition.x, node.GridPosition.y, out Cell linkedCell))
		        {
			        continue;
		        }
	            
		        linkedCell.RemoveNodeFromCell();
	        }
            
	        _instancedObjects.Remove(machineToSell.Controller);
	        Destroy(machineToSell.Controller.gameObject);

	        EconomyController.Instance.AddMoney(sellPrice);
	        machineToSell = null;
        }
    }
}