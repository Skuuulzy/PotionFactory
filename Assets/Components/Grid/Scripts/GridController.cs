using UnityEngine;
using CodeMonkey.Utils;
using System.Collections.Generic;
using Components.Machines;
using Sirenix.OdinInspector;
using System;
using Components.Items;

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
        
        [Header("Holders")]
        [SerializeField] private Transform _groundHolder;
        [SerializeField] private Transform _objectsHolder;
        [SerializeField] private Transform _obstacleHolder;

        [Header("Obstacles")]
        [SerializeField] private List<GameObject> _obstacleList;
        [SerializeField] private float _obstacleGenerationProbability;

        [Header("Extractor")]
        [SerializeField] private bool _addRandomExtractor;
        [SerializeField] private MachineTemplate _extractorMachine;
        [SerializeField] private List<ItemTemplate> _itemTemplateList;
		[SerializeField] private float _extractorGenerationProbability;

		// Grid
		private Grid _grid;
		private readonly List<MachineController> _instancedObjects = new ();
		
        // Preview
        private MachineController _currentMachineController;
        private int _currentRotation;
        private UnityEngine.Camera _camera;
        
        // Actions
        public Action<Machine> OnMachineAdded;
        public Action<Machine> OnMachineRemoved;
        

        // ------------------------------------------------------------------------- MONO -------------------------------------------------------------------------
        private void Start()
        {
            _camera = UnityEngine.Camera.main;
            InstantiateNewPreview();
            GenerateGrid();
        }

        private void Update()
        {
            MoveSelection();
            
            if (Input.GetMouseButton(1))
            {
                RemoveMachineFromGrid();

            }
            if (Input.GetMouseButton(0))
            {
                AddSelectedMachineToGrid();
            }
            if (Input.GetMouseButtonDown(2))
            {
                RotateSelection();
            }
        }
        
        // ------------------------------------------------------------------------- SELECTION -------------------------------------------------------------------------
        private void InstantiateNewPreview()
        {
            _currentMachineController = Instantiate(_machineControllerPrefab);
            _currentMachineController.InstantiatePreview(MachineManager.Instance.SelectedMachine, _cellSize);
            _currentMachineController.RotatePreview(_currentRotation);

            MachineManager.OnChangeSelectedMachine += UpdateSelection;
        }
        
        private void UpdateSelection(MachineTemplate newTemplate)
        {
	        Destroy(_currentMachineController.gameObject);

	        _currentMachineController = Instantiate(_machineControllerPrefab);
            _currentMachineController.InstantiatePreview(newTemplate, _cellSize);

            _currentRotation = 0;
        }

        private void DeleteSelection()
        {
           //_currentMachineController.DeletePreview();
		}
        
        private void MoveSelection()
        {
            if (!UtilsClass.ScreenToWorldPositionIgnoringUI(Input.mousePosition, _camera, out Vector3 worldMousePosition))
            {
                return;
            }

            // Update the object's position
            _currentMachineController.transform.position = worldMousePosition;
        }
        
        private void RotateSelection()
        {
            _currentRotation += 90;
            _currentRotation %= 360;
            _currentMachineController.RotatePreview(_currentRotation);
        }

        // ------------------------------------------------------------------------- INPUT HANDLERS -------------------------------------------------------------------------
        private void AddSelectedMachineToGrid()
        {
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
			foreach (var node in _currentMachineController.Machine.Nodes)
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

        private void AddMachineToGrid(MachineTemplate machine, Cell originCell)
        {
	        _instancedObjects.Add(_currentMachineController);
	        
            _currentMachineController.transform.position = _grid.GetWorldPosition(originCell.X, originCell.Y) + new Vector3(_cellSize / 2, 0, _cellSize / 2);
            _currentMachineController.transform.name = $"{machine.Name}_{_instancedObjects.Count}";
            _currentMachineController.transform.parent = _objectsHolder;
            
            // Adding nodes to the cells
            foreach (var node in _currentMachineController.Machine.Nodes)
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
            
            _currentMachineController.ConfirmPlacement();
            
            InstantiateNewPreview();
        }

        private void TryBindConnectedPort(Port port, Vector2Int neighbourPosition)
        {
	        if (_grid.TryGetCellByCoordinates(neighbourPosition.x , neighbourPosition.y, out Cell neighbourCell))
	        {
		        if (neighbourCell.ContainsObject)
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
			DeleteSelection();
			// Try to get the world position.
			if (!UtilsClass.ScreenToWorldPositionIgnoringUI(Input.mousePosition, _camera, out Vector3 worldMousePosition))
            {
                return;
            }

            // Try getting the cell
            if (!_grid.TryGetCellByPosition(worldMousePosition, out Cell chosenCell))
            {
                return;
            }

            // Check if the cell has an object
            if (!chosenCell.ContainsObject)
            {
                return;
            }
            
            //Destroy the machine associated to the node
            var machineToDestroy = chosenCell.Node.Machine;

            //Reset all cell linked to the machine.
            foreach (var node in chosenCell.Node.Machine.Nodes)
            {
	            if (!_grid.TryGetCellByCoordinates(node.GridPosition.x, node.GridPosition.y, out Cell linkedCell))
	            {
		            continue;
	            }
	            
	            linkedCell.RemoveNodeFromCell();
            }
            
            _instancedObjects.Remove(machineToDestroy.Controller);
            Destroy(machineToDestroy.Controller.gameObject);
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

            // Instantiate ground blocks
            for (int x = 0; x < _grid.GetWidth(); x++)
            {
                for (int z = 0; z < _grid.GetHeight(); z++)
                {
                    var tile = Instantiate(_groundTile, _grid.GetWorldPosition(x, z), Quaternion.identity, _groundHolder);
                    tile.transform.localScale = new Vector3(_cellSize, _cellSize, _cellSize);
                    tile.name = $"Cell ({x}, {z})";

                    if (!_addRandomExtractor)
                    {
	                    continue;
                    }
                    
                    bool isExtractor = false;

					if (x == 0 || x == _grid.GetWidth() - 1 || z == 0 || z == _grid.GetHeight() - 1)
                    {
					    isExtractor = GenerateExtractor(x, z);
                    }

                    if (!isExtractor)
                    {
						//Instantiate Obstacle
						GenerateObstacle(x, z);
					}
                    
                }
            }
        }
        
        public void ClearGrid()
        {
            foreach (var machineController in _instancedObjects)
            {
                Destroy(machineController.gameObject);
            }
            
            _grid.ClearCellsData();
            _instancedObjects.Clear();
        }

        private bool GenerateExtractor(int x, int z)
        {
	        // if (!(UnityEngine.Random.value <= _extractorGenerationProbability) || _itemTemplateList.Count == 0)
	        // {
		       //  return false;
	        // }
	        //
	        // ManageRotation(x, z);
	        // _grid.TryGetCellByCoordinates(x, z, out var cell);
	        // MachineController machineController = AddMachineToGrid(_extractorMachine, cell);
	        //
	        // if (machineController.Machine.Behavior is ExtractorMachineBehaviour extractor)
	        // {
		       //  ItemTemplate itemTemplate = _itemTemplateList[UnityEngine.Random.Range(0, _itemTemplateList.Count)];
		       //  extractor.Init(itemTemplate);
		       //  _itemTemplateList.Remove(itemTemplate);
	        //
		       //  //Reset current rotation
		       //  _currentRotation = 0;
		       //  return true;
	        // }

	        return false;
        }
        
        private void GenerateObstacle(int x, int z)
        {
	        if (!(UnityEngine.Random.value <= _obstacleGenerationProbability))
	        {
		        return;
	        }
	        
	        _grid.TryGetCellByCoordinates(x, z, out var cell);
				
	        var obstacle = Instantiate(_obstacleList[UnityEngine.Random.Range(0, _obstacleList.Count)], _obstacleHolder);
	        obstacle.transform.position = _grid.GetWorldPosition(cell.X, cell.Y) + new Vector3(_cellSize / 2, 0, _cellSize / 2);
	        obstacle.transform.localScale = new Vector3(_cellSize, _cellSize, _cellSize);
	        obstacle.transform.localRotation = Quaternion.Euler(new Vector3(0, -_currentRotation, 0));
				
	        cell.AddObstacleToCell(obstacle);
        }

        private void ManageRotation(int x, int z)
        {
	        if (x == 0)
	        {
		        _currentRotation = 0;
	        }
	        else if (x == _grid.GetWidth() - 1)
	        {
		        _currentRotation = 180;
	        }
	        else if (z == 0)
	        {
		        _currentRotation = 90;
	        }
	        else if (z == _grid.GetHeight() - 1)
	        {
		        _currentRotation = 270;
	        }

			_currentRotation %= 360;
		}
    }
}