using System;
using UnityEngine;
using CodeMonkey.Utils;
using System.Collections.Generic;
using Components.Machines;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

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
        [SerializeField] private MachineController _machineControllerPrefab;
        
        [Header("Holders")]
        [SerializeField] private Transform _textsHolder;
        [SerializeField] private Transform _objectsHolder;
        
        private Grid _grid;
        private readonly Dictionary<Cell, GameObject> _instancedObjects = new();

        private MachineController _currentMachineController;
        [SerializeField] private int _currentRotation;
        private UnityEngine.Camera _camera;

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
            _currentMachineController = Instantiate(_machineControllerPrefab);
            _currentMachineController.InstantiatePreview(MachineManager.Instance.SelectedMachine);

            MachineManager.OnChangeSelectedMachine += UpdateSelectionSelection;
        }
        
        private void UpdateSelectionSelection(MachineTemplate newTemplate)
        {
            _currentMachineController.InstantiatePreview(newTemplate);
            _currentRotation = 0;
            _currentMachineController.transform.rotation = Quaternion.identity;
        }
        
        private void MoveSelection()
        {
            // Get the mouse position in screen space
            Vector3 mousePosition = Input.mousePosition;

            // Calculate the z-depth based on the object's distance from the camera
            float zDepth = _camera.WorldToScreenPoint(transform.position).z;

            // Set the z-coordinate for depth
            mousePosition.z = zDepth;

            // Convert the screen position to world position
            Vector3 worldPosition = _camera.ScreenToWorldPoint(mousePosition);

            // Update the object's position
            _currentMachineController.transform.position = worldPosition;
        }
        
        private void RotateSelection()
        {
            _currentRotation += 90;
            _currentRotation %= 360;
            _currentMachineController.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -_currentRotation));
        }

        #region INPUT HANDLERS
        
        
        private void AddSelectedMachineToGrid()
        {
            var worldMousePosition = UtilsClass.GetWorldPositionFromUI_Perspective();
            
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

            //Instantiate a machine controller
            MachineController machineController = Instantiate(_machineControllerPrefab,
                _grid.GetWorldPosition(chosenCell.X, chosenCell.Y) +
                new Vector3(_grid.GetCellSize() / 2, _grid.GetCellSize() / 2, -_machineControllerPrefab.transform.localScale.z / 2),
                Quaternion.Euler(new Vector3(0, 0, -_currentRotation)), 
                _objectsHolder);

            // Set up the controller with the correct type;
            machineController.SetGridData(MachineManager.Instance.SelectedMachine, _grid.GetNeighboursByPosition(worldMousePosition), _currentRotation);
            
            //Add it to a dictionary to track it after
            _instancedObjects.Add(chosenCell, machineController.gameObject);
            
            // Renaming GO for debug purposes
            machineController.transform.name = $"{MachineManager.Instance.SelectedMachine.Name}_{_instancedObjects.Count}";
            
            //Set the AlreadyContainsMachine bool to true
            chosenCell.AddMachineToCell(machineController);
        }

        private void RemoveMachineFromGrid()
        {
            var worldMousePosition = UtilsClass.GetWorldPositionFromUI_Perspective();

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

            //Destroy the GameObject from the cell position
            Destroy(_instancedObjects[chosenCell]);
            _instancedObjects.Remove(chosenCell);

            //Reset cell state
            chosenCell.RemoveMachineFromCell();
        }
        
        #endregion INPUT HANDLERS

        #region GRID METHODS

        private void GenerateGrid()
        {
            if (_grid != null)
            {
                ClearGrid();
            }
            
            _grid = new Grid(_gridXValue, _gridYValue, _cellSize, _startPosition, _textsHolder, _showDebug);
        }
        
        public void ClearGrid()
        {
            foreach (var cell in _instancedObjects)
            {
                Destroy(cell.Value);
                cell.Key.RemoveMachineFromCell();
            }

            _instancedObjects.Clear();
        }

        #endregion
    }
}