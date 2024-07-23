using CodeMonkey.Utils;
using Components.Grid.Tile;
using System;
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

		[Header("Prefabs")]
		[SerializeField] private TileController _groundTilePrefab;

		[Header("Holders")]
		[SerializeField] private Transform _groundHolder;
		[SerializeField] private Transform _obstacleHolder;

		[Header("Tiles")]
		[SerializeField] private AllTilesController _allTilesController;

		[Header("Obstacles")]
		[SerializeField] private ObstacleController _obstacleController;

		private List<TileController> _tileInstantiateList;

		// Grid
		private Grid _grid;
		// Preview
		private TileController _currentTileController;

		private UnityEngine.Camera _camera;

		// ------------------------------------------------------------------------- MONO -------------------------------------------------------------------------
		private void Start()
		{
			_camera = UnityEngine.Camera.main;
			GenerateGrid();
			InstantiateNewPreview();
		}

		private void Update()
		{
			MoveSelection();

			//if (Input.GetMouseButton(1))
			//{
			//	RemoveMachineFromGrid();

			//}
			if (Input.GetMouseButton(0))
			{
				AddSelectedTileToGrid();
			}

		}



		// ------------------------------------------------------------------------- SELECTION -------------------------------------------------------------------------
		private void InstantiateNewPreview()
		{
			_currentTileController = Instantiate(_groundTilePrefab);
			TileManager.OnChangeSelectedTile += UpdateSelection;
		}

		private void UpdateSelection(TileTemplate newTemplate)
		{
			Destroy(_currentTileController.gameObject);

			_currentTileController = Instantiate(_groundTilePrefab);
			_currentTileController.InstantiatePreview(newTemplate, _cellSize);

		}
		private void MoveSelection()
		{
			if (!UtilsClass.ScreenToWorldPositionIgnoringUI(Input.mousePosition, _camera, out Vector3 worldMousePosition))
			{
				return;
			}

			// Update the object's position
			_currentTileController.transform.position = worldMousePosition;
		}

		// ------------------------------------------------------------------------- INPUT HANDLERS -------------------------------------------------------------------------
		private void AddSelectedTileToGrid()
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

			foreach(TileController tile in _tileInstantiateList)
			{
				if(tile.TileCoordinate[0] == chosenCell.X && tile.TileCoordinate[1] == chosenCell.Y)
				{
					TileController tileInstantiate = _allTilesController.GenerateTileByPrefab(chosenCell.X, chosenCell.Y, _grid, _groundHolder, _cellSize, _currentTileController);
					_tileInstantiateList.Add(tileInstantiate);

					_tileInstantiateList.Remove(tile);
					Destroy(tile.gameObject);
					return;
				}
			}


		}

		// ------------------------------------------------------------------------- GENERATE GRID -------------------------------------------------------------------------
		public void GenerateGrid()
		{
			if (_grid != null)
			{
				ClearGrid();
			}

			_grid = new Grid(_gridXValue, _gridYValue, _cellSize, _startPosition, _groundHolder, false);
			_tileInstantiateList = new List<TileController>();
			_allTilesController.SelectATileType();

			// Instantiate ground blocks
			for (int x = 0; x < _grid.GetWidth(); x++)
			{
				for (int z = 0; z < _grid.GetHeight(); z++)
				{
					TileController tile = _allTilesController.GenerateTile(x, z, _grid, _groundHolder, _cellSize);
					_tileInstantiateList.Add(tile);
					if (tile.IsWater == false)
					{
						if (x != 1 && x != _grid.GetWidth() - 2 && z != 1 && z != _grid.GetHeight() - 2)
						{
							//Instantiate Obstacle
							_grid.TryGetCellByCoordinates(x, z, out var chosenCell);
							_obstacleController.GenerateObstacle(_grid, chosenCell, _obstacleHolder, _cellSize);
						}
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
