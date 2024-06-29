using UnityEngine;
using CodeMonkey.Utils;
using System.Collections.Generic;
using Components.Machines;

namespace Components.Grid
{
    public class GridGenerator : MonoBehaviour
    {
        private Grid _grid;
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
        
        private readonly Dictionary<Cell, GameObject> _instancedObjects = new();

        #region MONO

        private void Start()
        {
            GenerateGrid();
        }

        private void Update()
        {
            if (Input.GetMouseButton(1))
            {
                HandleRightClick();
            }
            if (Input.GetMouseButton(0))
            {
                HandleLeftClick();
            }
        }

        #endregion MONO

        #region INPUT HANDLERS
        
        private void HandleLeftClick()
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

            //Cell the value of the grid to 1 (useless but nice feedback)
            _grid.SetValue(UtilsClass.GetWorldPositionFromUI_Perspective(), 1);

            //Instantiate a machine controller
            MachineController machineController = Instantiate(_machineControllerPrefab,
                _grid.GetWorldPosition(chosenCell.X, chosenCell.Y) +
                new Vector3(_grid.GetCellSize() / 2, _grid.GetCellSize() / 2, -_machineControllerPrefab.transform.localScale.z / 2), Quaternion.identity, _objectsHolder);

            // Set up the controller with the correct type;
            machineController.Init(MachineManager.Instance.SelectedMachine);
            
            //Add it to a dictionary to track it after
            _instancedObjects.Add(chosenCell, machineController.gameObject);
            
            // Renaming GO for debug purposes
            machineController.transform.name = $"{MachineManager.Instance.SelectedMachine.Template.Type}_{_instancedObjects.Count}";
            
            //Set the AlreadyContainsMachine bool to true
            chosenCell.AddMachineToCell();
        }

        private void HandleRightClick()
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
            
            //Cell the value of the grid to 0 (useless but nice feedback)
            _grid.SetValue(UtilsClass.GetWorldPositionFromUI_Perspective(), 0);

            //Destroy the GameObject from the cell position
            Destroy(_instancedObjects[chosenCell]);
            _instancedObjects.Remove(chosenCell);

            //Reset cell state
            chosenCell.RemoveMachineFromCell();
        }
        
        #endregion INPUT HANDLERS

        #region GRID METHODS

        public void GenerateGrid()
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

            _grid.ResetAllValue();
            _instancedObjects.Clear();
        }

        #endregion
    }
}