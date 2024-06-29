using System;
using UnityEngine;
using CodeMonkey.Utils;
using System.Collections.Generic;

namespace Components.Grid
{
    public class Grid
    {
        public readonly Action<int,int> OnCellValueChanged;

        private readonly int _width;
        private readonly int _height;
        private readonly float _cellSize;
        private readonly Vector3 _originPosition;
        private readonly int[,] _gridArray;
        private readonly List<Cell> _cellsList;

        public Grid(int width, int height, float cellSize, Vector3 originPosition, Transform parentTransform, bool showDebug)
        {
            _width = width;
            _height = height;
            _cellSize = cellSize;
            _originPosition = originPosition;

            _gridArray = new int[width, height];
            _cellsList = new List<Cell>();
            
            if (showDebug)
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
                        _cellsList.Add(cell);

                        debugTextArray[x][y] = UtilsClass.CreateWorldText(_gridArray[x, y].ToString(), parentTransform, GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f, 30, Color.white,
                            TextAnchor.MiddleCenter);
                        Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                        Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                    }
                }

                Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);

                OnCellValueChanged += (x, y) =>
                {
                    debugTextArray[x][y].text = _gridArray[x, y].ToString();
                };
            }
        }

        #region HELPERS

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
            return new Vector3(x, y) * _cellSize + _originPosition;
        }

        private void GetCellCoordinates(Vector3 worldPosition, out int x, out int y)
        {
            x = Mathf.FloorToInt((worldPosition - _originPosition).x / _cellSize);
            y = Mathf.FloorToInt((worldPosition - _originPosition).y / _cellSize);
        }

        #endregion HELPERS

        #region VALUES

        public void SetValue(int x, int y, int value)
        {
            if (x >= 0 && y >= 0 && x < _width && y < _height)
            {
                _gridArray[x, y] = value;
                OnCellValueChanged?.Invoke(x,y);
            }
        }

        public void SetValue(Vector3 worldPosition, int value)
        {
            int x, y;
            GetCellCoordinates(worldPosition, out x, out y);
            SetValue(x, y, value);
        }

        public void ResetAllValue()
        {
            for (int x = 0; x < _gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < _gridArray.GetLength(1); y++)
                {
                    SetValue(x, y, 0);
                }
            }
        }

        public int GetValue(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < _width && y < _height)
            {
                return _gridArray[x, y];
            }
            else
            {
                return 0;
            }
        }

        public int GetValue(Vector3 worldPosition)
        {
            int x, y;
            GetCellCoordinates(worldPosition, out x, out y);
            return GetValue(x, y);
        }

        #endregion VALUES

        #region GET CELLS

        /// <summary>
        /// From a set of coordinates return a cell if one is found.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="foundCell">The cell potentially found on the coordinates.</param>
        /// <returns>True if a cell is found, otherwise false.</returns>
        public bool TryGetCellByCoordinates(int x, int y, out Cell foundCell)
        {
            foreach (Cell cell in _cellsList)
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

        #endregion GET CELLS
    }
}