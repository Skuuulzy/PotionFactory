using System;
using CodeMonkey.Utils;
using Components.Grid.Decorations;
using Components.Grid.Obstacle;
using Components.Grid.Tile;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using VComponent.Tools.EventSystem;

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
		[SerializeField] private TacticaleViewController _tacticalViewPrefab;
		[SerializeField] private ObstacleController _obstaclePrefab;
		[SerializeField] private DecorationController _decorationPrefab;
		[SerializeField] private GameObject _waterPlanePrefab;

		[Header("Holders")]
		[SerializeField] private Transform _groundHolder;
		[SerializeField] private Transform _obstacleHolder;
		[SerializeField] private Transform _decorationHolder;
		[SerializeField] private Transform _TacticalViewHolder;

		[Header("Tiles")]
		[SerializeField] private AllTilesController _allTilesController;

		[Header("Obstacles")]
		[SerializeField] private AllObstaclesController _allObstacleController;

		[Header("Decorations")]
		[SerializeField] private AllDecorationsController _allDecorationController;

		[Header("Options")]
		[SerializeField] private float _rotationSpeed = 300f;

		[Header("Modifying Events")]
		[SerializeField] private FloatEventChannel _scalingEvent;
		[SerializeField] private FloatEventChannel _yPositionEvent;
		private bool _lockScaling;
		private bool _lockUpAndDown;

		private Vector3 _lastCellPosition = new(-1, -1, -1);
		private List<Cell> _cellList;

		// Grid
		private Grid _grid;
		// Preview
		private GridObjectController _currentGridObjectController;

		private Camera _camera;

		private string _jsonString;
		private string _fileName;
		private bool _freePlacement;
		private bool _cleanMode;
		private float _cleanRadius = 1f;
		private float _objectYPosition;

		public bool CleanMode => _cleanMode;
		public float CleanRadius => _cleanRadius;

		public static string MapsPath => Path.Combine(Application.dataPath, "JsonData/Maps/");


		// ------------------------------------------------------------------------- MONO -------------------------------------------------------------------------
		private void Start()
		{
			_camera = Camera.main;
			GenerateGrid();
			InstantiateNewPreview();

		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Tab))
			{
				DisplayTacticalView(true);
			}
			else if(Input.GetKeyUp(KeyCode.Tab))
			{
				DisplayTacticalView(false);
			}
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R))
			{
				RotateSelectionBy90Degrees();
			}
			else if (Input.GetKey(KeyCode.R))
			{
				RotateSelection();
			}
			else if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.T))
			{
				LockScaling(!_lockScaling);
			}
			else if(Input.GetKey(KeyCode.T))
			{
				ScaleSelection();
			}
			else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.LeftControl))
			{
				LockUpAndDown(!_lockUpAndDown);
			}
			else if (Input.GetKey(KeyCode.LeftControl))
			{
				UpAndDownSelection();
			}
			else
			{
				MoveSelection();
			}

			if (Input.GetMouseButtonDown(0))
			{
				if (_currentGridObjectController != null && _freePlacement && !_cleanMode)
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

				else if(_currentGridObjectController != null && _freePlacement)
				{
					return;
				}

				AddSelectedObjectToGrid();	
			}
			if (Input.GetMouseButton(1))
			{
				DeletePreview();
			}

		}

		private void DisplayTacticalView(bool value)
		{
			_TacticalViewHolder.gameObject.SetActive(value);
			_obstacleHolder.gameObject.SetActive(!value);
			_decorationHolder.gameObject.SetActive(!value);
		}

		private void DeletePreview()
		{
			if (_currentGridObjectController != null)
			{
				Destroy(_currentGridObjectController.gameObject);
				_currentGridObjectController = null;
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
			if (_currentGridObjectController != null)
			{
				Destroy(_currentGridObjectController.gameObject);
			}

			_currentGridObjectController = Instantiate(_tilePrefab) ;
			var tileController = _currentGridObjectController as TileController;
			tileController.SetTileType(tileTemplate.TileType);
			_currentGridObjectController.InstantiatePreview(tileTemplate, _cellSize);

			OnScaleChange(1);
			OnYPositionChange(0);
		}

		private void UpdateObstacleSelection(ObstacleTemplate obstacleTemplate)
		{
			if (_currentGridObjectController != null)
			{
				Destroy(_currentGridObjectController.gameObject);
			}

			_currentGridObjectController = Instantiate(_obstaclePrefab);
			var currentObstacleController = _currentGridObjectController as ObstacleController;
			currentObstacleController.SetObstacleType(obstacleTemplate.ObstacleType);
			_currentGridObjectController.InstantiatePreview(obstacleTemplate, _cellSize);

			OnScaleChange(1);
			OnYPositionChange(0);
		}

		private void UpdateDecorationSelection(DecorationTemplate decorationTemplate)
		{
			if (_currentGridObjectController != null)
			{
				Destroy(_currentGridObjectController.gameObject);
			}

			_currentGridObjectController = Instantiate(_decorationPrefab);
			var currentDecorationController = _currentGridObjectController as DecorationController;
			currentDecorationController.SetDecorationType(decorationTemplate.DecorationType);
			_currentGridObjectController.InstantiatePreview(decorationTemplate, _cellSize);

			OnScaleChange(1);
			OnYPositionChange(0);
		}

		private void MoveSelection()
		{
			if (!UtilsClass.ScreenToWorldPositionIgnoringUI(Input.mousePosition, _camera, out Vector3 worldMousePosition))
			{
				return;
			}
			
			if (_grid.TryGetCellByPosition(worldMousePosition, out Cell cell))
			{
				Vector3 position = worldMousePosition;

				if (!_freePlacement)
				{
					position = cell.GetCenterPosition(_startPosition);
					// Preventing the computation when staying on the same cell.
					if (position == _lastCellPosition)
					{
						return;
					}

					_lastCellPosition = position;
				}
				

				// Update the object's position
				if (_currentGridObjectController == null)
				{
					return;
				}
				else if (_currentGridObjectController is TileController)
				{
					_currentGridObjectController.transform.position = position;
				}
				//All others grid object controller
				else
				{
					_currentGridObjectController.transform.position = position + new Vector3(0, _currentGridObjectController.transform.position.y);
				}
			}
			


		}

		/// <summary>
		/// Rotates the currently selected object (decoration or obstacle) based on mouse movement while holding the R key.
		/// </summary>
		private void RotateSelection()
		{
			if (_currentGridObjectController == null || _currentGridObjectController is TileController)
			{
				return;
			}

			// Capture horizontal mouse delta
			float mouseDeltaX = Input.GetAxis("Mouse X");


			float rotationAngle = mouseDeltaX * _rotationSpeed * Time.deltaTime;

			// Apply rotation
			_currentGridObjectController.transform.Rotate(Vector3.up, rotationAngle);
			
		}

		/// <summary>
		/// Rotates the currently selected object (decoration or obstacle) by 90 degrees when Shift + R is pressed.
		/// </summary>
		private void RotateSelectionBy90Degrees()
		{
			if (_currentGridObjectController == null || _currentGridObjectController is TileController)
			{
				return;
			}

			// Determine the rotation step (90 degrees around the Y axis)
			float rotationAngle = 90f;

			// Apply rotation
			_currentGridObjectController.transform.Rotate(Vector3.up, rotationAngle);
		}
		
		/// <summary>
		/// Modifies the local scale of the currently selected object (decoration or obstacle) based on vertical mouse movement while holding the T key.
		/// </summary>
		private void ScaleSelection()
		{
			if (_currentGridObjectController == null || _currentGridObjectController is TileController)
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
			OnScaleChange(_currentGridObjectController.transform.localScale.x * scaleFactor);

		}
		public void ScaleSelection(float scaleFactor)
		{
			_currentGridObjectController.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
		}

		private void OnScaleChange(float value)
		{
			if (_lockScaling)
			{
				return;
			}
			_scalingEvent?.Invoke(value);
		}

		public void LockScaling(bool value)
		{
			_lockScaling = value;
		}

		public void UpAndDownSelection()
		{
			if (_currentGridObjectController == null || _currentGridObjectController is TileController)
			{
				return;
			}

			float mouseDeltaY = Input.GetAxis("Mouse Y");

			float moveSpeed = 0.1f; 
			_objectYPosition = mouseDeltaY * moveSpeed;
			Vector3 newPosition = new Vector3();

			newPosition = _currentGridObjectController.transform.position;
			newPosition.y += _objectYPosition;


			OnYPositionChange(newPosition.y);
		}
		public void OnYPositionChange(float value)
		{
			if (_lockUpAndDown)
			{
				return;
			}
			_yPositionEvent?.Invoke(value);
		}

		public void UpAndDownSelection(float newPosition)
		{
			_currentGridObjectController.transform.position = new Vector3(_currentGridObjectController.transform.position.x,newPosition, _currentGridObjectController.transform.position.z);
		}

		public void LockUpAndDown(bool value)
		{
			_lockUpAndDown = value;
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

			if(_currentGridObjectController == null)
			{
				return;
			}

			if(_currentGridObjectController is TileController tileController)
			{
				if(chosenCell.TileController != null)
				{
					Destroy(chosenCell.TileController.gameObject);
				}
				TileController tileInstantiate = _allTilesController.GenerateTileFromPrefab(chosenCell, _grid, _groundHolder, _cellSize, tileController);
				chosenCell.AddTileToCell(tileInstantiate);

				return;
			}

			if (_currentGridObjectController is ObstacleController obstacleController)
			{
				if(chosenCell.ObstacleController != null)
				{
					Destroy(chosenCell.ObstacleController.gameObject);
				}
				ObstacleController obstacleInstantiate = _allObstacleController.GenerateObstacleFromPrefab(_grid, chosenCell, _obstacleHolder, _cellSize, obstacleController, _freePlacement);
				chosenCell.AddObstacleToCell(obstacleInstantiate);
			}
			else if (_currentGridObjectController is DecorationController decorationController)
			{
				if (chosenCell.DecorationControllers != null && !_freePlacement && chosenCell.DetectDecorationOnCell(decorationController))
				{
					return;
				}

				DecorationController decorationInstantiate = _allDecorationController.GenerateDecorationFromPrefab(_grid, chosenCell, _decorationHolder, _cellSize, decorationController, _freePlacement);
				chosenCell.AddDecorationToCell(decorationInstantiate);
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
			if(_currentGridObjectController != null)
			{
				Destroy(_currentGridObjectController.gameObject);
				_currentGridObjectController = null;
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
							_allObstacleController.GenerateObstacle(_grid, chosenCell, _obstacleHolder, _cellSize);
						}
					}
					_cellList.Add(chosenCell);
				}
			}
			
			// Instantiate water plane
			if (_waterPlanePrefab)
			{
				var waterPlane = Instantiate(_waterPlanePrefab, transform);
				waterPlane.transform.position = _startPosition + new Vector3(_grid.GetWidth() / 2f, 0, _grid.GetHeight() / 2f);
				waterPlane.transform.localScale = new Vector3(_grid.GetWidth() / 10f, 1, _grid.GetHeight() / 10f);
			}
			else
			{
				Debug.LogError("No water prefab found.");
			}
		}

		private void GenerateGridFromTemplate(List<SerializedCell> serializedCellList)
		{
			if (_grid != null)
			{
				ClearGrid();
			}
			_grid = new Grid(_gridXValue, _gridYValue, _cellSize, _startPosition, _groundHolder, false, serializedCellList.ToArray());
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
						if(serializedCell.TileType == TileType.WATER)
						{
							TacticaleViewController tacticalView = Instantiate(_tacticalViewPrefab, _TacticalViewHolder);
							tacticalView.transform.position = _grid.GetWorldPosition(chosenCell.X, chosenCell.Y) + new Vector3(_cellSize / 2, 0, _cellSize / 2);
						}
						_allTilesController.GenerateTileFromType(chosenCell, _grid, _groundHolder, _cellSize, serializedCell.TileType);
					}

					if (serializedCell.ObstacleType != ObstacleType.NONE)
					{
						TacticaleViewController tacticalView = Instantiate(_tacticalViewPrefab, _TacticalViewHolder);
						tacticalView.transform.position = _grid.GetWorldPosition(chosenCell.X, chosenCell.Y) + new Vector3(_cellSize / 2, 0, _cellSize / 2);

						// Lire les coordonnées de la décoration
						float[] positionArray = serializedCell.ObstaclePositions;
						Vector3 obstaclePositions = new Vector3(positionArray[0], positionArray[1], positionArray[2]);

						// Lire la rotation de l'obstacle
						float[] rotationArray = serializedCell.ObstacleRotation;
						Quaternion obstacleRotation = new Quaternion(rotationArray[0], rotationArray[1], rotationArray[2], rotationArray[3]);

						// Lire l'échelle locale de l'obstacle
						float[] scaleArray = serializedCell.ObstacleScale;
						Vector3 obstacleScale = new Vector3(scaleArray[0], scaleArray[1], scaleArray[2]);

						// Générer l'obstacle avec la rotation et l'échelle récupérées
						_allObstacleController.GenerateObstacleFromType(chosenCell, _grid, _obstacleHolder, _cellSize, serializedCell.ObstacleType, obstaclePositions, obstacleRotation,	obstacleScale);
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

			File.WriteAllText(Path.Combine(MapsPath, _fileName), _jsonString);
			AssetDatabase.Refresh();
			Debug.Log($"Map saved to: {Path.Combine(MapsPath, _fileName)}");
		}

		public void LoadMap()
		{
			string jsonContent = File.ReadAllText(Path.Combine(MapsPath, _fileName));

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
			}

			GenerateGridFromTemplate(serializedCellArray.ToList());	
		}
		
		public static bool TryLoadRandomMap(out SerializedCell[] serializedCells)
		{
			// Get all potential maps.
			var mapFileNames = GetAllMapFileNames();
			
			// Select a random file
			int randomIndex = Random.Range(0, mapFileNames.Count);

			if (TryLoadMapAt(randomIndex, out var cells))
			{
				serializedCells = cells;
				return true;
			}

			serializedCells = null;
			return false;
		}

		public static bool TryLoadMapAt(int index, out SerializedCell[] serializedCells)
		{
			// Get all potential maps.
			var mapFileNames = GetAllMapFileNames();
			
			if (mapFileNames.Count == 0 || index > mapFileNames.Count - 1)
			{
				Debug.LogError($"No map files were found at index: {index}.");
				serializedCells = null;
				return false;
			}
			
			string jsonContent = File.ReadAllText(Path.Combine(MapsPath, mapFileNames[index]));
			serializedCells = JsonConvert.DeserializeObject<SerializedCell[]>(jsonContent);

			if (serializedCells == null || serializedCells.Length == 0)
			{
				Debug.LogError("No cells were loaded. Check the JSON format.");
				return false;
			}

			for (var i = 0; i < serializedCells.Length; i++)
			{
				var cell = serializedCells[i];

				if (cell != null) 
					continue;
				
				Debug.LogError("A null cell was found during loading.");
				return false;
			}

			Debug.Log($"Map : {mapFileNames[index]} successfully loaded");
			return true;
		}

		public static List<string> GetAllMapFileNames()
		{
			List<string> mapFileNames = new List<string>();

			if (!Directory.Exists(MapsPath))
			{
				Debug.LogError($"The directory '{MapsPath}' does not exist.");
				return mapFileNames;
			}
			
			DirectoryInfo info = new DirectoryInfo(MapsPath);
			FileInfo[] fileInfo = info.GetFiles();

			for (var i = 0; i < fileInfo.Length; i++)
			{
				var file = fileInfo[i];
				
				// Ignore .meta files
				if (file.Extension.Equals(".meta", StringComparison.OrdinalIgnoreCase))
					continue;

				mapFileNames.Add(file.Name);
			}

			return mapFileNames;
		}
	}
}