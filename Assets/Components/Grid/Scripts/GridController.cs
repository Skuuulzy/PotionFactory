using UnityEngine;
using CodeMonkey.Utils;
using System.Collections.Generic;
using Components.Machines;
using Sirenix.OdinInspector;
using System;
using Components.Items;
using Components.Machines.Behaviors;

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
        [SerializeField] private MachinePreviewController _machinePreviewControllerPrefab;
        
        [Header("Holders")]
        [SerializeField] private Transform _groundHolder;
        [SerializeField] private Transform _objectsHolder;
        [SerializeField] private Transform _obstacleHolder;

        [Header("Obstacles")]
        [SerializeField] private List<GameObject> _obstacleList;
        [SerializeField] private float _obstacleGenerationProbability;

        [Header("Extractor")]
        [SerializeField] private MachineTemplate _extractorMachine;
        [SerializeField] private List<ItemTemplate> _itemTemplateList;
		[SerializeField] private float _extractorGenerationProbability;



		private Grid _grid;
        private readonly Dictionary<Cell, GameObject> _instancedObjects = new();
        
        private MachinePreviewController _currentMachinePreviewController;
        private int _currentRotation;
        private UnityEngine.Camera _camera;


        public Action<Machine> OnMachineAdded;
        public Action<Machine> OnMachineRemoved;

        #region MONO

        private void Start()
        {
            _camera = UnityEngine.Camera.main;
            InstantiateSelection();
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

        #endregion MONO

        // ------------------------------------------------------------------------- SELECTION -------------------------------------------------------------------------
        
        private void InstantiateSelection()
        {
            _currentMachinePreviewController = Instantiate(_machinePreviewControllerPrefab);
            _currentMachinePreviewController.InstantiatePreview(MachineManager.Instance.SelectedMachine, _cellSize);

            MachineManager.OnChangeSelectedMachine += UpdateSelection;
        }
        
        private void UpdateSelection(MachineTemplate newTemplate)
        {
            _currentMachinePreviewController.InstantiatePreview(newTemplate, _cellSize);
            _currentRotation = 0;
            _currentMachinePreviewController.transform.rotation = Quaternion.identity;
        }

        private void DeleteSelection()
        {
            _currentMachinePreviewController.DeletePreview();
		}
        
        private void MoveSelection()
        {
            if (!UtilsClass.ScreenToWorldPositionIgnoringUI(Input.mousePosition, _camera, out Vector3 worldMousePosition))
            {
                return;
            }

            // Update the object's position
            _currentMachinePreviewController.transform.position = worldMousePosition;
        }
        
        private void RotateSelection()
        {
            _currentRotation += 90;
            _currentRotation %= 360;
            _currentMachinePreviewController.transform.rotation = Quaternion.Euler(new Vector3(0, -_currentRotation, 0));
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

			// Check if the cell has no object
			if (chosenCell.ContainsObject)
			{
				return;
			}
			AddMachineToGrid(MachineManager.Instance.SelectedMachine, chosenCell);
        }

        private MachineController AddMachineToGrid(MachineTemplate machine, Cell chosenCell)
        {

            //Instantiate a machine controller
            MachineController machineController = Instantiate(_machineControllerPrefab, _objectsHolder);
            machineController.transform.position = _grid.GetWorldPosition(chosenCell.X, chosenCell.Y) + new Vector3(_cellSize / 2, 0, _cellSize / 2);
            machineController.transform.localScale = new Vector3(_cellSize, _cellSize, _cellSize);
            machineController.transform.localRotation = Quaternion.Euler(new Vector3(0, -_currentRotation, 0));
            machineController.transform.name = $"{machine.Name}_{_instancedObjects.Count}";
            
            // Set up the controller with the correct type;
            machineController.SetGridData(machine, _grid.GetNeighboursByPosition(chosenCell), _currentRotation);
            
            //Add it to a dictionary to track it after
            _instancedObjects.Add(chosenCell, machineController.gameObject);
            
            //Set the AlreadyContainsMachine bool to true
            chosenCell.AddMachineToCell(machineController);

            //Call AddedMachine action
            OnMachineAdded?.Invoke(machineController.Machine);

            return machineController;
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

            //Call RemovedMachine action
            OnMachineRemoved?.Invoke(chosenCell.MachineController.Machine);

            //Destroy the GameObject from the cell position
            Destroy(_instancedObjects[chosenCell]);
            _instancedObjects.Remove(chosenCell);

            //Reset cell state
            chosenCell.RemoveMachineFromCell();
            
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
            foreach (var cell in _instancedObjects)
            {
                Destroy(cell.Value);
                cell.Key.RemoveMachineFromCell();
            }

            _instancedObjects.Clear();

            //foreach (Transform groundTile in _groundHolder)
            //{
            //    Destroy(groundTile.gameObject);
            //}
        }

        private bool GenerateExtractor(int x, int z)
        {
			float rand = UnityEngine.Random.value;
			if (rand <= _extractorGenerationProbability && _itemTemplateList.Count != 0)
            {
                ManageRotation(x, z);
				_grid.TryGetCellByCoordinates(x, z, out var cell);
				MachineController machineController = AddMachineToGrid(_extractorMachine, cell);
                if(machineController.Machine.Behavior is ExtractorMachineBehaviour)
                {
                    ItemTemplate itemTemplate = _itemTemplateList[UnityEngine.Random.Range(0, _itemTemplateList.Count)];
					(machineController.Machine.Behavior as ExtractorMachineBehaviour).Init(itemTemplate);
                    _itemTemplateList.Remove(itemTemplate);

                    //Reset current rotation
                    _currentRotation = 0; 
                    return true;
				}
			}
			return false;
        }

        private void ManageRotation(int x, int z)
        {
            if(x == 0)
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
            else if( z == _grid.GetHeight() - 1 )
            {
				_currentRotation = 270;
			}

			_currentRotation %= 360;
		}

		private void GenerateObstacle(int x, int z)
        {
			float rand = UnityEngine.Random.value;
			if (rand <= _obstacleGenerationProbability)
			{
				_grid.TryGetCellByCoordinates(x, z, out var cell);
				var obstacle = Instantiate(_obstacleList[UnityEngine.Random.Range(0, _obstacleList.Count)], _obstacleHolder);
				obstacle.transform.position = _grid.GetWorldPosition(cell.X, cell.Y) + new Vector3(_cellSize / 2, 0, _cellSize / 2);
				obstacle.transform.localScale = new Vector3(_cellSize, _cellSize, _cellSize);
				obstacle.transform.localRotation = Quaternion.Euler(new Vector3(0, -_currentRotation, 0));
				cell.AddObstacleToCell(obstacle);
			}
		}
    }
}