using UnityEngine;
using CodeMonkey.Utils;
using System.Collections.Generic;

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

        [SerializeField] private GameObject _objectToInstantiate;

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

            //Create a GameObject to the cell position and attached it to 3D parent Transform => need to change the game object by the machine that we want to add
            GameObject go = Instantiate(_objectToInstantiate,
                _grid.GetWorldPosition(chosenCell.X, chosenCell.Y) +
                new Vector3(_grid.GetCellSize() / 2, _grid.GetCellSize() / 2, -_objectToInstantiate.transform.localScale.z / 2), Quaternion.identity, _objectsHolder);

            //Add it to a dictionary to track it after
            _instancedObjects.Add(chosenCell, go);

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