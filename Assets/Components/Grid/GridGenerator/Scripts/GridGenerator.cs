using CodeMonkey.Utils;
using Components.Grid.Obstacle;
using Components.Grid.Tile;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
		[SerializeField] private TileController _tilePrefab;
		[SerializeField] private ObstacleController _obstaclePrefab;

		[Header("Holders")]
		[SerializeField] private Transform _groundHolder;
		[SerializeField] private Transform _obstacleHolder;

		[Header("Tiles")]
		[SerializeField] private AllTilesController _allTilesController;

		[Header("Obstacles")]
		[SerializeField] private AllObstaclesController _allObstacleController;

		[Header("TilesGenerated")]
		private List<TileController> _tileInstantiateList;
		private List<ObstacleController> _obstacleInstantiateList;

		// Grid
		private Grid _grid;
		// Preview
		private TileController _currentTileController;
		private ObstacleController _currentObstacleController;

		private UnityEngine.Camera _camera;

		private string _jsonString;

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
			_currentTileController = Instantiate(_tilePrefab);
			TileManager.OnChangeSelectedTile += UpdateTileSelection;
			ObstacleManager.OnChangeSelectedObstacle += UpdateObstacleSelection;

		}

		private void UpdateTileSelection(TileTemplate tileTemplate)
		{
			Destroy(_currentTileController.gameObject);
			Destroy(_currentObstacleController.gameObject);

			_currentTileController = Instantiate(_tilePrefab);
			_currentTileController.InstantiatePreview(tileTemplate, _cellSize);

		}

		private void UpdateObstacleSelection(ObstacleTemplate obstacleTemplate)
		{
			Destroy(_currentTileController.gameObject);
			Destroy(_currentObstacleController.gameObject);

			_currentObstacleController = Instantiate(_obstaclePrefab);
			_currentObstacleController.InstantiatePreview(obstacleTemplate, _cellSize);
		}

		private void MoveSelection()
		{
			if (!UtilsClass.ScreenToWorldPositionIgnoringUI(Input.mousePosition, _camera, out Vector3 worldMousePosition))
			{
				return;
			}

			// Update the object's position
			if(_currentTileController != null)
			{
				_currentTileController.transform.position = worldMousePosition;
			}
			else if (_currentObstacleController != null)
			{
				_currentObstacleController.transform.position = worldMousePosition;
			}
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

			if(_currentTileController != null)
			{
				foreach (TileController tile in _tileInstantiateList)
				{
					if (tile.Cell == chosenCell)
					{
						TileController tileInstantiate = _allTilesController.GenerateTileFromPrefab(chosenCell, _grid, _groundHolder, _cellSize, _currentTileController);
						_tileInstantiateList.Add(tileInstantiate);

						_tileInstantiateList.Remove(tile);
						Destroy(tile.gameObject);
						return;
					}
				}
			}

			else if( _currentObstacleController != null)
			{
				foreach (ObstacleController obstacle in _obstacleInstantiateList)
				{
					if (obstacle.Cell == chosenCell)
					{
						ObstacleController obstacleController = _allObstacleController.GenerateObstacleFromPrefab(_grid, chosenCell,  _obstacleHolder, _cellSize, _currentObstacleController);
						_obstacleInstantiateList.Add(obstacleController);
						_obstacleInstantiateList.Remove(obstacle);
						Destroy(obstacle.gameObject);
						return;
					}
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
					_grid.TryGetCellByCoordinates(x, z, out var chosenCell);
					TileController tile = _allTilesController.GenerateTile(chosenCell, _grid, _groundHolder, _cellSize);
					_tileInstantiateList.Add(tile);
					if (tile.TileType != TileType.WATER)
					{
						if (x != 1 && x != _grid.GetWidth() - 2 && z != 1 && z != _grid.GetHeight() - 2)
						{
							_allObstacleController.GenerateObstacle(_grid, chosenCell, _obstacleHolder, _cellSize);
						}
					}
				}
			}
		}

		private void GenerateGridFromTemplate(List<TileController> tileControllerList)
		{
			if (_grid != null)
			{
				ClearGrid();
			}
			_grid = new Grid(_gridXValue, _gridYValue, _cellSize, _startPosition, _groundHolder, false);
			//_grid.TryGetCellByCoordinates(x, z, out var chosenCell);
			//_tileInstantiateList = tileControllerList;
			//foreach(TileController tile in tileControllerList)
			//{
			//	_allTilesController.GenerateTileFromPrefab(chosenCell, _grid, _groundHolder, _cellSize, tile);
			//}
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

		// ------------------------------------------------------------------------ SAVE AND LOAD MAP -------------------------------------------------------------
		public void SaveMap()
		{

			_jsonString = JsonHelper.ToJson(_tileInstantiateList.ToArray(), true);
			Debug.Log(Application.persistentDataPath);
			System.IO.File.WriteAllText(Application.persistentDataPath + "/Map.json", _jsonString);
		}

		public void LoadMap()
		{
			TileController[] tileControllerArray = JsonHelper.FromJson<TileController>(System.IO.File.ReadAllText(Application.persistentDataPath + "/Map.json"));
			Debug.Log(tileControllerArray[0]);
			GenerateGridFromTemplate(tileControllerArray.ToList());
		}
	}

}
