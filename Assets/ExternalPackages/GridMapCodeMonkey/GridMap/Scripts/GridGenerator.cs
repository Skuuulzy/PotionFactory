using UnityEngine;
using CodeMonkey.Utils;
using System.Collections.Generic;

public class GridGenerator : MonoBehaviour
{

    private Grid _grid;
    [SerializeField] private int _gridXValue = 64;
    [SerializeField] private int _gridYValue = 64;
    [SerializeField] private float _cellSize = 10;
    [SerializeField] private Vector3 _startPosition = new Vector3(0, 0);
    [SerializeField] private Transform _parentTransform;
    [SerializeField] private Transform _3DparentTransform;
    [SerializeField] private GameObject _objectToInstantiate;

    private bool _leftClickIsDraging = false;
    private bool _rightClickIsDraging = false;
    private Dictionary<Cell,GameObject> _objectsInstantiateDictionary = new Dictionary<Cell, GameObject>();

	private int _tickCount = 0;

	private int _tickMax = 10;

	private void Start()
    {
		_grid = new Grid(_gridXValue, _gridYValue, _cellSize, _startPosition, _parentTransform);
    }

    private void Update() 
    {
		//Right click button press
		if (Input.GetMouseButtonDown(1))
		{
			_rightClickIsDraging = true;

		}
		//Right click button release
		else if (Input.GetMouseButtonUp(1))
		{
			_rightClickIsDraging = false;
		}
		//If player press right click button => handle right click
		if (_rightClickIsDraging == true)
		{
			HandleRightClick();
		}

		//Right click button press
		if (Input.GetMouseButtonDown(0))
		{
            _leftClickIsDraging = true;
			
		}
		//Right click button release
		else if (Input.GetMouseButtonUp(0))
		{
			_leftClickIsDraging = false;
		}
		//If player press left click button => handle left click
		if (_leftClickIsDraging == true)
        {
			HandleLeftClick();
		}
	}

    private void HandleLeftClick() 
    {
        //Get the cell that player clicked on
        Cell chosenCell = _grid.GetCellByPosition(UtilsClass.GetWorldPositionFromUI_Perspective());

		//Check if the cell is not null and if it isn't contain a machine already
        if(chosenCell != null && chosenCell.AlreadyContainsMachine == false)
        {
			//Cell the value of the grid to 1 (useless but nice feedback)
			_grid.SetValue(UtilsClass.GetWorldPositionFromUI_Perspective(), 1);

			//Create a GameObject to the cell position and attached it to 3D parent Transform => need to change the gameobject by the machine that we want to add
			GameObject go = Instantiate(_objectToInstantiate, _grid.GetWorldPosition(chosenCell.CoordinateX, chosenCell.CoordinateY) + new Vector3(_grid.GetCellSize() / 2, _grid.GetCellSize() / 2,  - _objectToInstantiate.transform.localScale.z/2), Quaternion.identity, _3DparentTransform);

			//Add it to a dictinary to track it after
			_objectsInstantiateDictionary.Add(chosenCell, go);

			//Set the AlreadyContainsMachine bool to true
			chosenCell.AddMachineToCell();
        }
    }

    private void HandleRightClick()
    {
		//Get the cell that player clicked on
		Cell chosenCell = _grid.GetCellByPosition(UtilsClass.GetWorldPositionFromUI_Perspective());

		//Check if the cell is not null and if it contains a machine already
		if (chosenCell != null && chosenCell.AlreadyContainsMachine == true)
        {
			//Cell the value of the grid to 0 (useless but nice feedback)
			_grid.SetValue(UtilsClass.GetWorldPositionFromUI_Perspective(), 0);

			//Destroy the GameObject from the cell position
			Destroy(_objectsInstantiateDictionary[chosenCell]);
            _objectsInstantiateDictionary.Remove(chosenCell);

			//Set the AlreadyContainsMachine bool to false
			chosenCell.RemoveMachineFromCell();
		}
	}


	public void ClearGrid()
	{
		foreach(var  cell in _objectsInstantiateDictionary)
		{
			Destroy(cell.Value);
			cell.Key.RemoveMachineFromCell();
		}

		_grid.ResetAllValue();
		_objectsInstantiateDictionary.Clear();
	}

}
