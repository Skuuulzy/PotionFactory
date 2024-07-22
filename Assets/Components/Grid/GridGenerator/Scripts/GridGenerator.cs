using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Grid.Generator
{
	public class GridGenerator : MonoBehaviour
	{
		[Header("Generation Parameters")]
		[SerializeField] private int _gridXValue = 64;
		[SerializeField] private int _gridYValue = 64;
		[SerializeField] private float _cellSize = 10;
		[SerializeField] private Vector3 _startPosition = new(0, 0);

		[Header("Holders")]
		[SerializeField] private Transform _groundHolder;
		[SerializeField] private Transform _obstacleHolder;

		[Header("Tiles")]
		[SerializeField] private TileController _tileController;

		[Header("Obstacles")]
		[SerializeField] private ObstacleController _obstacleController;

		// Grid
		private Grid _grid;

		private UnityEngine.Camera _camera;

		// ------------------------------------------------------------------------- MONO -------------------------------------------------------------------------
		private void Start()
		{
			_camera = UnityEngine.Camera.main;
			GenerateGrid();
		}

		public void GenerateGrid()
		{
			if (_grid != null)
			{
				ClearGrid();
			}

			_grid = new Grid(_gridXValue, _gridYValue, _cellSize, _startPosition, _groundHolder, false);
			_tileController.SelectATileType();

			// Instantiate ground blocks
			for (int x = 0; x < _grid.GetWidth(); x++)
			{
				for (int z = 0; z < _grid.GetHeight(); z++)
				{
					bool cellIsWater = _tileController.GenerateTile(x, z, _grid, _groundHolder, _cellSize);

					if (x != 1 && x != _grid.GetWidth() - 2 && z != 1 && z != _grid.GetHeight() - 2)
					{
						//Instantiate Obstacle
						_grid.TryGetCellByCoordinates(x, z, out var chosenCell);
						_obstacleController.GenerateObstacle(_grid, chosenCell, _obstacleHolder, _cellSize);
					}
				}
			}
		}

		private void ClearGrid()
		{
			foreach (Transform groundTile in _groundHolder)
			{
				Destroy(groundTile.gameObject);
			}
			foreach (Transform obstacleTile in _obstacleHolder)
			{
				Destroy(obstacleTile.gameObject);
			}

			_grid.ClearCellsData();
		}
	}
}
