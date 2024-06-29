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

        [SerializeField] private GameObject _objectToInstantiate;

        [Header("Holders")]
        [SerializeField] private Transform _textsHolder;
        [SerializeField] private Transform _objectsHolder;
        
        
        private bool _leftClickIsDragging;
        private bool _rightClickIsDragging;
        private readonly Dictionary<Cell, GameObject> _instancedObjects = new();

        private void Start()
        {
            _grid = new Grid(_gridXValue, _gridYValue, _cellSize, _startPosition, _textsHolder);
        }

        private void Update()
        {
            //Right click button press
            if (Input.GetMouseButtonDown(1))
            {
                _rightClickIsDragging = true;
            }
            //Right click button release
            else if (Input.GetMouseButtonUp(1))
            {
                _rightClickIsDragging = false;
            }

            //If player press right click button => handle right click
            if (_rightClickIsDragging)
            {
                HandleRightClick();
            }

            //Right click button press
            if (Input.GetMouseButtonDown(0))
            {
                _leftClickIsDragging = true;
            }
            //Right click button release
            else if (Input.GetMouseButtonUp(0))
            {
                _leftClickIsDragging = false;
            }

            //If player press left click button => handle left click
            if (_leftClickIsDragging)
            {
                HandleLeftClick();
            }
        }

        private void HandleLeftClick()
        {
            //Get the cell that player clicked on
            Cell chosenCell = _grid.GetCellByPosition(UtilsClass.GetWorldPositionFromUI_Perspective());

            //Check if the cell is not null and if it isn't contain a machine already
            if (chosenCell != null && chosenCell.ContainsObject == false)
            {
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
        }

        private void HandleRightClick()
        {
            //Get the cell that player clicked on
            Cell chosenCell = _grid.GetCellByPosition(UtilsClass.GetWorldPositionFromUI_Perspective());

            //Check if the cell is not null and if it contains a machine already
            if (chosenCell != null && chosenCell.ContainsObject)
            {
                //Cell the value of the grid to 0 (useless but nice feedback)
                _grid.SetValue(UtilsClass.GetWorldPositionFromUI_Perspective(), 0);

                //Destroy the GameObject from the cell position
                Destroy(_instancedObjects[chosenCell]);
                _instancedObjects.Remove(chosenCell);

                //Set the AlreadyContainsMachine bool to false
                chosenCell.RemoveMachineFromCell();
            }
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
    }
}