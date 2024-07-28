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
		private List<Cell> _cellList;
		private List<TileController> _tileInstantiateList;

		// Grid
		private Grid _grid;
		// Preview
		private TileController _currentTileController;
		private ObstacleController _currentObstacleController;

		private UnityEngine.Camera _camera;

		private string _jsonString;
		private string _fileName;

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

			if (Input.GetMouseButton(1))
			{
				RemoveObstacleFromGrid();
			}

			if (Input.GetMouseButton(0))
			{
				AddSelectedTileOrObstacleToGrid();
			}

		}



		// ------------------------------------------------------------------------- SELECTION -------------------------------------------------------------------------
		private void InstantiateNewPreview()
		{
			_currentTileController = Instantiate(_tilePrefab);
			_currentObstacleController = Instantiate(_obstaclePrefab);

			TileManager.OnChangeSelectedTile += UpdateTileSelection;
			ObstacleManager.OnChangeSelectedObstacle += UpdateObstacleSelection;

		}

		private void UpdateTileSelection(TileTemplate tileTemplate)
		{
			if(_currentTileController != null)
			{
				Destroy(_currentTileController.gameObject);
			}
			
			if( _currentObstacleController != null )
			{
				Destroy(_currentObstacleController.gameObject);
			}

			_currentTileController = Instantiate(_tilePrefab);
			_currentTileController.SetTileType(tileTemplate.TileType);
			_currentTileController.InstantiatePreview(tileTemplate, _cellSize);

		}

		private void UpdateObstacleSelection(ObstacleTemplate obstacleTemplate)
		{
			if (_currentTileController != null)
			{
				Destroy(_currentTileController.gameObject);
			}

			if (_currentObstacleController != null)
			{
				Destroy(_currentObstacleController.gameObject);
			}

			_currentObstacleController = Instantiate(_obstaclePrefab);

			_currentObstacleController.SetObstacleType(obstacleTemplate.ObstacleType);
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
		private void AddSelectedTileOrObstacleToGrid()
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
				foreach(TileController tileController in _tileInstantiateList)
				{
					if(tileController == chosenCell.TileController)
					{
						Destroy(tileController.gameObject);
						_tileInstantiateList.Remove(tileController);
						break;
					}
				}

				TileController tileInstantiate = _allTilesController.GenerateTileFromPrefab(chosenCell, _grid, _groundHolder, _cellSize, _currentTileController);
				_tileInstantiateList.Add(tileInstantiate);
				chosenCell.AddTileToCell(tileInstantiate);

				return;
			}

			else if( _currentObstacleController != null)
			{


				ObstacleController obstacleController = _allObstacleController.GenerateObstacleFromPrefab(_grid, chosenCell, _obstacleHolder, _cellSize, _currentObstacleController);
				chosenCell.AddObstacleToCell(obstacleController);
				return;
			}

		}

		private void RemoveObstacleFromGrid()
		{
			if(_currentTileController != null)
			{
				Destroy(_currentTileController.gameObject);
				_currentTileController = null;	
			}
			else if (_currentObstacleController != null)
			{
				Destroy(_currentObstacleController.gameObject);
				_currentObstacleController = null;
			}

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

			if (chosenCell.ContainsObstacle == true)
			{
				Destroy(chosenCell.ObstacleController.gameObject);
				chosenCell.RemoveObstacleFromCell();
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
			_cellList = new List<Cell>();
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
					_cellList.Add(chosenCell);
				}
			}
		}

		private void GenerateGridFromTemplate(List<SerializedCell> serializedCellList)
		{
			if (_grid != null)
			{
				ClearGrid();
			}
			_grid = new Grid(_gridXValue, _gridYValue, _cellSize, _startPosition, _groundHolder, false, serializedCellList);
			_tileInstantiateList = new List<TileController>();
			_cellList = new List<Cell>();
			// Instantiate ground blocks
			for (int x = 0; x < _grid.GetWidth(); x++)
			{
				for (int z = 0; z < _grid.GetHeight(); z++)
				{
					_grid.TryGetCellByCoordinates(x, z, out var chosenCell);
					SerializedCell serializeCell = serializedCellList.Find(cell => cell.X == x && cell.Y == z);

					if(serializeCell.TileType != TileType.NONE)
					{
						TileController tile = _allTilesController.GenerateTileFromType(chosenCell, _grid, _groundHolder, _cellSize, serializeCell.TileType);
						_tileInstantiateList.Add(tile);
					}

					if(serializeCell.ObstacleType != ObstacleType.NONE)
					{
						ObstacleController obstacle = _allObstacleController.GenerateObstacleFromType(chosenCell, _grid, _obstacleHolder, _cellSize, serializeCell.ObstacleType);
					}

					_cellList.Add(chosenCell);
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

		// ------------------------------------------------------------------------ SAVE AND LOAD MAP -------------------------------------------------------------

		public void SetFileName(string fileName)
		{
			_fileName = fileName;
		}
		public void SaveMap()
		{
			SerializedCell[] serializeCellArray = new SerializedCell[_cellList.Count];
			for (int i = 0; i < serializeCellArray.Length; i++)
			{
				SerializedCell cell = new SerializedCell(_cellList[i]);
				serializeCellArray[i] = cell;
			}

			_jsonString = JsonHelper.ToJson(serializeCellArray, true);
			Debug.Log(Application.persistentDataPath);
			System.IO.File.WriteAllText(Application.persistentDataPath + $"/{_fileName}.json", _jsonString);
		}

		public void LoadMap()
		{
			SerializedCell[] serializedCellArray = JsonHelper.FromJson<SerializedCell>(System.IO.File.ReadAllText(Application.persistentDataPath + $"/{_fileName}.json"));
			GenerateGridFromTemplate(serializedCellArray.ToList());
		}
	}

}
