using CodeMonkey.Utils;
using Components.Grid.Decorations;
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
		[SerializeField] private DecorationController _decorationPrefab;

		[Header("Holders")]
		[SerializeField] private Transform _groundHolder;
		[SerializeField] private Transform _obstacleHolder;
		[SerializeField] private Transform _decorationHolder;

		[Header("Tiles")]
		[SerializeField] private AllTilesController _allTilesController;

		[Header("Obstacles")]
		[SerializeField] private AllObstaclesController _allObstacleController;

		[Header("Decorations")]
		[SerializeField] private AllDecorationsController _allDecorationController;


		[Header("Options")]
		[SerializeField] private float _rotationSpeed = 300f;

		private List<Cell> _cellList;

		// Grid
		private Grid _grid;
		// Preview
		private TileController _currentTileController;
		private ObstacleController _currentObstacleController;
		private DecorationController _currentDecorationController;

		private Camera _camera;

		private string _jsonString;
		private string _fileName;
		private bool _freePlacement;
		private bool _cleanMode;
		private float _cleanRadius = 1f;

		public bool CleanMode => _cleanMode;
		public float CleanRadius => _cleanRadius;

		// ------------------------------------------------------------------------- MONO -------------------------------------------------------------------------
		private void Start()
		{
			_camera = Camera.main;
			GenerateGrid();
			InstantiateNewPreview();
		}

		private void Update()
		{
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R))
			{
				RotateSelectionBy90Degrees();
			}
			else if (Input.GetKey(KeyCode.R))
			{
				RotateSelection();
			}
			else if(Input.GetKey(KeyCode.T))
			{
				ScaleSelection();
			}
			else
			{
				MoveSelection();
			}

			if (Input.GetMouseButtonDown(0))
			{
				if (_currentDecorationController != null && _freePlacement && !_cleanMode)
				{
					AddSelectedObjectToGrid();
				}
			}

			if ( Input.GetMouseButton(0))
			{
				if (_cleanMode)
				{
					RemoveObjectFromGrid();
					return;
				}

				else if(_currentDecorationController != null && _freePlacement)
				{
					return;
				}

				AddSelectedObjectToGrid();	
			}

		}



		// ------------------------------------------------------------------------- SELECTION -------------------------------------------------------------------------
		private void InstantiateNewPreview()
		{
			TileManager.OnChangeSelectedTile += UpdateTileSelection;
			ObstacleManager.OnChangeSelectedObstacle += UpdateObstacleSelection;
			DecorationManager.OnChangeSelectedDecoration += UpdateDecorationSelection;

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

			if (_currentDecorationController != null)
			{
				Destroy(_currentDecorationController.gameObject);
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

			if (_currentDecorationController != null)
			{
				Destroy(_currentDecorationController.gameObject);
			}

			_currentObstacleController = Instantiate(_obstaclePrefab);

			_currentObstacleController.SetObstacleType(obstacleTemplate.ObstacleType);
			_currentObstacleController.InstantiatePreview(obstacleTemplate, _cellSize);
		}

		private void UpdateDecorationSelection(DecorationTemplate decorationTemplate)
		{
			if (_currentTileController != null)
			{
				Destroy(_currentTileController.gameObject);
			}

			if (_currentObstacleController != null)
			{
				Destroy(_currentObstacleController.gameObject);
			}

			if (_currentDecorationController != null)
			{
				Destroy(_currentDecorationController.gameObject);
			}

			_currentDecorationController = Instantiate(_decorationPrefab);

			_currentDecorationController.SetDecorationType(decorationTemplate.DecorationType);
			_currentDecorationController.InstantiatePreview(decorationTemplate, _cellSize);
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
			else if (_currentDecorationController != null)
			{
				_currentDecorationController.transform.position = worldMousePosition;
			}
		}

		/// <summary>
		/// Rotates the currently selected object (decoration or obstacle) based on mouse movement while holding the R key.
		/// </summary>
		private void RotateSelection()
		{
			if (_currentObstacleController == null && _currentDecorationController == null)
			{
				return;
			}

			// Capture horizontal mouse delta
			float mouseDeltaX = Input.GetAxis("Mouse X");


			float rotationAngle = mouseDeltaX * _rotationSpeed * Time.deltaTime;

			// Apply rotation
			if (_currentObstacleController != null)
			{
				_currentObstacleController.transform.Rotate(Vector3.up, rotationAngle);
			}
			else if (_currentDecorationController != null)
			{
				_currentDecorationController.transform.Rotate(Vector3.up, rotationAngle);
			}
		}

		/// <summary>
		/// Rotates the currently selected object (decoration or obstacle) by 90 degrees when Shift + R is pressed.
		/// </summary>
		private void RotateSelectionBy90Degrees()
		{
			if (_currentObstacleController == null && _currentDecorationController == null)
			{
				return;
			}

			// Determine the rotation step (90 degrees around the Y axis)
			float rotationAngle = 90f;

			// Apply rotation
			if (_currentObstacleController != null)
			{
				_currentObstacleController.transform.Rotate(Vector3.up, rotationAngle);
			}
			else if (_currentDecorationController != null)
			{
				_currentDecorationController.transform.Rotate(Vector3.up, rotationAngle);
			}
		}


		/// <summary>
		/// Modifies the local scale of the currently selected object (decoration or obstacle) based on vertical mouse movement while holding the T key.
		/// </summary>
		private void ScaleSelection()
		{
			if (_currentObstacleController == null && _currentDecorationController == null)
			{
				return;
			}

			// Capture vertical mouse delta
			float mouseDeltaY = Input.GetAxis("Mouse Y");

			// Determine scaling factor (adjust sensitivity as needed)
			float scaleSpeed = 0.1f; // Scale speed factor
			float scaleFactor = 1 + mouseDeltaY * scaleSpeed;

			// Clamp scale factor to avoid negative or zero scale
			scaleFactor = Mathf.Clamp(scaleFactor, 0.1f, 3f);

			// Apply scaling
			if (_currentObstacleController != null)
			{
				_currentObstacleController.transform.localScale *= scaleFactor;
			}
			else if (_currentDecorationController != null)
			{
				_currentDecorationController.transform.localScale *= scaleFactor;
			}
		}



		// ------------------------------------------------------------------------- INPUT HANDLERS -------------------------------------------------------------------------
		private void AddSelectedObjectToGrid()
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
				if(chosenCell.TileController != null)
				{
					Destroy(chosenCell.TileController.gameObject);
				}
				TileController tileInstantiate = _allTilesController.GenerateTileFromPrefab(chosenCell, _grid, _groundHolder, _cellSize, _currentTileController);
				chosenCell.AddTileToCell(tileInstantiate);

				return;
			}

			else if( _currentObstacleController != null)
			{
				if(chosenCell.ObstacleController != null)
				{
					Destroy(chosenCell.ObstacleController.gameObject);
				}
				ObstacleController obstacleController = _allObstacleController.GenerateObstacleFromPrefab(_grid, chosenCell, _obstacleHolder, _cellSize, _currentObstacleController);
				chosenCell.AddObstacleToCell(obstacleController);
				return;
			}

			else if (_currentDecorationController != null)
			{
				if (chosenCell.DecorationControllers != null && !_freePlacement && chosenCell.DetectDecorationOnCell(_currentDecorationController))
				{
					return;
				}

				DecorationController decorationController = _allDecorationController.GenerateDecorationFromPrefab(_grid, chosenCell, _decorationHolder, _cellSize, _currentDecorationController, _freePlacement, worldMousePosition);
				chosenCell.AddDecorationToCell(decorationController);
				return;
			}

		}

		private void RemoveObjectsInCircle(Vector3 center, float radius)
		{
			// Get all the cells within the circle.
			List<Cell> cellsInCircle = _grid.GetCellsInCircle(center, radius);

			foreach (var cell in cellsInCircle)
			{
				if (_cleanMode)
				{
					// Remove the obstacle if it exists.
					if (cell.ContainsObstacle)
					{
						Destroy(cell.ObstacleController.gameObject);
						cell.RemoveObstacleFromCell();
					}

					// Remove all decorations in the cell.
					if (cell.DecorationControllers != null && cell.DecorationControllers.Count > 0)
					{
						foreach (var decoration in cell.DecorationControllers.ToArray()) // Use ToArray to avoid issues while modifying the collection.
						{
							Destroy(decoration.gameObject);
							cell.RemoveDecorationFromCell(decoration);
						}
					}
				}
			}
		}

		private void RemoveObjectFromGrid()
		{
			// Try to get the world position ignoring UI.
			if (!UtilsClass.ScreenToWorldPositionIgnoringUI(Input.mousePosition, _camera, out Vector3 worldMousePosition))
			{
				return;
			}

			// Remove objects in the circle.
			RemoveObjectsInCircle(worldMousePosition, _cleanRadius);
		}

		public void ChangeFreePlacementMode(bool value)
		{
			_freePlacement = value;
		}

		public void ChangeCleanMode(bool value)
		{
			_cleanMode = value;

			if (_cleanMode)
			{
				DestroyCurrentController();
			}
		}



		private void DestroyCurrentController()
		{
			if (_currentTileController != null)
			{
				Destroy(_currentTileController.gameObject);
				_currentTileController = null;
			}

			else if (_currentObstacleController != null)
			{
				Destroy(_currentObstacleController.gameObject);
				_currentObstacleController = null;
			}

			else if (_currentDecorationController != null)
			{
				Destroy(_currentDecorationController.gameObject);
				_currentDecorationController = null;
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
			_allTilesController.SelectATileType();

			// Instantiate ground blocks
			for (int x = 0; x < _grid.GetWidth(); x++)
			{
				for (int z = 0; z < _grid.GetHeight(); z++)
				{
					_grid.TryGetCellByCoordinates(x, z, out var chosenCell);
					TileController tile = _allTilesController.GenerateTile(chosenCell, _grid, _groundHolder, _cellSize);
					if (tile.TileType != TileType.WATER)
					{
						if (x != 1 && x != _grid.GetWidth() - 2 && z != 1 && z != _grid.GetHeight() - 2)
						{
							ObstacleController obstacleController = _allObstacleController.GenerateObstacle(_grid, chosenCell, _obstacleHolder, _cellSize);
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
			_cellList = new List<Cell>();
			// Instantiate ground blocks
			for (int x = 0; x < _grid.GetWidth(); x++)
			{
				for (int z = 0; z < _grid.GetHeight(); z++)
				{
					_grid.TryGetCellByCoordinates(x, z, out var chosenCell);
					SerializedCell serializedCell = serializedCellList.Find(cell => cell.X == x && cell.Y == z);

					if (serializedCell.TileType != TileType.NONE)
					{
						TileController tile = _allTilesController.GenerateTileFromType(chosenCell, _grid, _groundHolder, _cellSize, serializedCell.TileType);
					}

					if (serializedCell.ObstacleType != ObstacleType.NONE)
					{
						// Lire la rotation de l'obstacle
						float[] rotationArray = serializedCell.ObstacleRotation;
						Quaternion obstacleRotation = new Quaternion(rotationArray[0], rotationArray[1], rotationArray[2], rotationArray[3]);

						// Lire l'échelle locale de l'obstacle
						float[] scaleArray = serializedCell.ObstacleScale;
						Vector3 obstacleScale = new Vector3(scaleArray[0], scaleArray[1], scaleArray[2]);

						// Générer l'obstacle avec la rotation et l'échelle récupérées
						ObstacleController obstacle = _allObstacleController.GenerateObstacleFromType(chosenCell, _grid, _obstacleHolder, _cellSize, serializedCell.ObstacleType, obstacleRotation,	obstacleScale);
					}

					if (serializedCell.DecorationPositions != null && serializedCell.DecorationPositions.Count > 0)
					{
						for (int i = 0; i < serializedCell.DecorationPositions.Count; i++)
						{
							// Lire les coordonnées de la décoration
							float[] positionArray = serializedCell.DecorationPositions[i];
							Vector3 decorationPosition = new Vector3(positionArray[0], positionArray[1], positionArray[2]);

							// Lire la rotation de la décoration
							float[] rotationArray = serializedCell.DecorationRotations[i];
							Quaternion decorationRotation = new Quaternion(rotationArray[0], rotationArray[1], rotationArray[2], rotationArray[3]);

							// Lire l'échelle locale de la décoration
							float[] scaleArray = serializedCell.DecorationScales[i];
							Vector3 decorationScale = new Vector3(scaleArray[0], scaleArray[1], scaleArray[2]);

							// Générer la décoration avec la position, la rotation, et l'échelle récupérées
							_allDecorationController.GenerateDecorationFromType(chosenCell,	_decorationHolder, serializedCell.DecorationTypes[i], decorationPosition, decorationRotation, decorationScale);
						}
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
			foreach (Transform decorationTile in _decorationHolder)
			{
				Destroy(decorationTile.gameObject);
			}


			_grid.ClearNodes();
		}

		public void ChangeRadiusValue(float radiusValue)
		{
			_cleanRadius = radiusValue / 10f;
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

			_jsonString = JsonConvert.SerializeObject(serializeCellArray, Formatting.Indented);
			Debug.Log(Application.persistentDataPath);
			System.IO.File.WriteAllText(Application.persistentDataPath + $"/{_fileName}", _jsonString);
		}

		public void LoadMap()
		{
			string jsonContent = System.IO.File.ReadAllText(Application.persistentDataPath + $"/{_fileName}");

			Debug.Log("JSON Loaded: " + jsonContent);

			// Désérialisation avec Newtonsoft.Json
			SerializedCell[] serializedCellArray = JsonConvert.DeserializeObject<SerializedCell[]>(jsonContent);

			if (serializedCellArray == null || serializedCellArray.Length == 0)
			{
				Debug.LogError("No cells were loaded. Check the JSON format.");
				return;
			}

			Debug.Log($"Loaded {serializedCellArray.Length} cells.");

			foreach (var cell in serializedCellArray)
			{
				if (cell == null)
				{
					Debug.LogError("A null cell was found during loading.");
				}
				else
				{
					Debug.Log($"Loaded cell at position ({cell.X}, {cell.Y}) with TileType {cell.TileType}");
				}
			}

			GenerateGridFromTemplate(serializedCellArray.ToList());	
		}
	}

}
