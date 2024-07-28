using UnityEngine;
using CodeMonkey.Utils;
using System.Collections.Generic;

namespace Components.Grid
{
    public class Grid
    {
        private readonly int _width;
        private readonly int _height;
        private readonly float _cellSize;
        private readonly Vector3 _originPosition;
        private readonly int[,] _gridArray;
        private readonly List<Cell> _cells;
        
        // ------------------------------------------------------------------------- CONSTRUCTOR -------------------------------------------------------------------------
        public Grid(int width, int height, float cellSize, Vector3 originPosition, Transform parentTransform, bool showDebug)
        {
            _width = width;
            _height = height;
            _cellSize = cellSize;
            _originPosition = originPosition;

            _gridArray = new int[width, height];
            _cells = new List<Cell>();

            for (int x = 0; x < _gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < _gridArray.GetLength(1); y++)
                {
                    //Create a new cell and add it to cell list
                    Cell cell = new Cell(x, y, cellSize, false);
                    _cells.Add(cell);
                }
            }

            if (showDebug)
            {
                DrawGridDebug(width, height, cellSize, parentTransform);
            }
        }

        public Grid(int width, int height, float cellSize, Vector3 originPosition, Transform parentTransform, bool showDebug, List<SerializedCell> serializedCellList)
		{
            _width = width;
            _height = height;
            _cellSize = cellSize;
            _originPosition = originPosition;

            _gridArray = new int[width, height];
            _cells = new List<Cell>();

            for (int i = 0; i < serializedCellList.Count; i++)
			{
                SerializedCell serializedCell = serializedCellList[i];
                //Create a new cell and add it to cell list
                Cell cell = new Cell(serializedCell.X, serializedCell.Y, cellSize, serializedCell.ContainsObject);
                _cells.Add(cell);
            }

            if (showDebug)
            {
                DrawGridDebug(width, height, cellSize, parentTransform);
            }
        }

        // ------------------------------------------------------------------------- GRID INFOS -------------------------------------------------------------------------
        public int GetWidth()
        {
            return _width;
        }

        public int GetHeight()
        {
            return _height;
        }

        public float GetCellSize()
        {
            return _cellSize;
        }

        public Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(x, 0, y) * _cellSize + _originPosition;
        }

        private void GetCellCoordinates(Vector3 worldPosition, out int x, out int y)
        {
            x = Mathf.FloorToInt((worldPosition - _originPosition).x / _cellSize);
            y = Mathf.FloorToInt((worldPosition - _originPosition).z / _cellSize);
        }
        
        // ------------------------------------------------------------------------- CELLS -------------------------------------------------------------------------
        /// <summary>
        /// From a set of coordinates return a cell if one is found.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="foundCell">The cell potentially found on the coordinates.</param>
        /// <returns>True if a cell is found, otherwise false.</returns>
        public bool TryGetCellByCoordinates(int x, int y, out Cell foundCell)
        {
            foreach (Cell cell in _cells)
            {
                if (cell.X == x && cell.Y == y)
                {
                    foundCell = cell;
                    return true;
                }
            }

            foundCell = null;
            return false;
        }

        /// <summary>
        /// From a world position return a cell if one is found.
        /// </summary>
        /// <param name="worldPosition">The position you want to test.</param>
        /// <param name="foundCell">The cell potentially found on the world position.</param>
        /// <returns>True if a cell is found, otherwise false.</returns>
        public bool TryGetCellByPosition(Vector3 worldPosition, out Cell foundCell)
        {
            GetCellCoordinates(worldPosition, out var x, out var y);

            if (TryGetCellByCoordinates(x, y, out var cell))
            {
                foundCell = cell;
                return true;
            }

            foundCell = null;
            return false;
        }



		public void ClearCellsData()
        {
            foreach (var cell in _cells)
            {
                cell.RemoveNodeFromCell();
                cell.RemoveObstacleFromCell();
            }
        }
        
        // ------------------------------------------------------------------------- DEBUG -------------------------------------------------------------------------
        private void DrawGridDebug(int width, int height, float cellSize, Transform parentTransform)
        {
            TextMesh[][] debugTextArray = new TextMesh[width][];
            for (int index = 0; index < width; index++)
            {
                debugTextArray[index] = new TextMesh[height];
            }

            for (int x = 0; x < _gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < _gridArray.GetLength(1); y++)
                {
                    //Create a new cell and add it to cellLList
                    Cell cell = new Cell(x, y, cellSize, false);
                    _cells.Add(cell);

                    var worldPosition = GetWorldPosition(x, y) + new Vector3(_cellSize / 2, 0, _cellSize / 2);

                    debugTextArray[x][y] = UtilsClass.CreateWorldText($"({x},{y})", parentTransform, worldPosition, 60, Color.white, TextAnchor.MiddleCenter);
                    debugTextArray[x][y].transform.rotation = Quaternion.Euler(90, 0, 0);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, float.PositiveInfinity);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, float.PositiveInfinity);
                }
            }

            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, float.PositiveInfinity);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, float.PositiveInfinity);
        }
    }
}