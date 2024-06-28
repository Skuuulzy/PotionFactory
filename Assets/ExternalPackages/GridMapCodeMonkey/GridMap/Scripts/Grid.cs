using System;
using UnityEngine;
using CodeMonkey.Utils;
using System.Collections.Generic;

public class Grid {

    public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
    public class OnGridValueChangedEventArgs : EventArgs {
        public int x;
        public int y;
    }

    private int _width;
    private int _height;
    private float _cellSize;
    private Vector3 _originPosition;
    private int[,] _gridArray;
    [SerializeField] private List<Cell> _cellsList;

    public Grid(int width, int height, float cellSize, Vector3 originPosition, Transform parentTransform) {
        this._width = width;
        this._height = height;
        this._cellSize = cellSize;
        this._originPosition = originPosition;

        _gridArray = new int[width, height];
        _cellsList = new List<Cell>();
        bool showDebug = true;
        if (showDebug) 
        {
            TextMesh[,] debugTextArray = new TextMesh[width, height];

            for (int x = 0; x < _gridArray.GetLength(0); x++) {
                for (int y = 0; y < _gridArray.GetLength(1); y++) {

                    //Create a new cell and add it to cellLList
                    Cell cell = new Cell(x, y, cellSize, false);
                    _cellsList.Add(cell);

					debugTextArray[x, y] = UtilsClass.CreateWorldText(_gridArray[x, y].ToString(), parentTransform, GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f, 30, Color.white, TextAnchor.MiddleCenter);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                }
            }

            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);

            OnGridValueChanged += (object sender, OnGridValueChangedEventArgs eventArgs) => {
                debugTextArray[eventArgs.x, eventArgs.y].text = _gridArray[eventArgs.x, eventArgs.y].ToString();
            };
        }
    }

    public int GetWidth() {
        return _width;
    }

    public int GetHeight() {
        return _height;
    }

    public float GetCellSize() {
        return _cellSize;
    }

    public Vector3 GetWorldPosition(int x, int y) {
        return new Vector3(x, y) * _cellSize + _originPosition;
    }

    private void GetXY(Vector3 worldPosition, out int x, out int y) {
        x = Mathf.FloorToInt((worldPosition - _originPosition).x / _cellSize);
        y = Mathf.FloorToInt((worldPosition - _originPosition).y / _cellSize);
    }

    public void SetValue(int x, int y, int value) {
        if (x >= 0 && y >= 0 && x < _width && y < _height) {
            _gridArray[x, y] = value;
            if (OnGridValueChanged != null) OnGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, y = y });
        }
    }

    public void SetValue(Vector3 worldPosition, int value) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetValue(x, y, value);
    }

    public int GetValue(int x, int y) {
        if (x >= 0 && y >= 0 && x < _width && y < _height) {
            return _gridArray[x, y];
        } else {
            return 0;
        }
    }

    public int GetValue(Vector3 worldPosition) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetValue(x, y);
    }

	#region Cells
	public Cell GetCellByPosition(int x, int y)
    {
        foreach(Cell cell in _cellsList)
        {
            if(cell.CoordinateX == x && cell.CoordinateY == y)
            {
                return cell;
            }
        }

        return null;
    }

	public Cell GetCellByPosition(Vector3 worldPosition)
	{
		int x, y;
		GetXY(worldPosition, out x, out y);
		return GetCellByPosition(x, y);
	}
	#endregion Cells

}

public class Cell
{
    private int _coordinateX;
    private int _coordinateY;
    private float _cellSize;
    private bool _alreadyContainsMachine;

    public int CoordinateX => _coordinateX;
    public int CoordinateY => _coordinateY;
    public float CellSize => _cellSize;
    public bool AlreadyContainsMachine => _alreadyContainsMachine;

    public Cell(int coordinateX, int coordinateY, float cellSize, bool alreadyContainsMachine)
	{
		_coordinateX = coordinateX;
		_coordinateY = coordinateY;
        _cellSize = cellSize;
		_alreadyContainsMachine = alreadyContainsMachine;
	}

    public void AddMachineToCell()
    {
        _alreadyContainsMachine = true;
    }

    public void RemoveMachineFromCell()
    {
        _alreadyContainsMachine = false;
    }
}
