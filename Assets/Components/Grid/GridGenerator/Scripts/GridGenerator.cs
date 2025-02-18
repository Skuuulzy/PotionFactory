using System;
using CodeMonkey.Utils;
using Components.Grid.Decorations;
using Components.Grid.Obstacle;
using Components.Grid.Tile;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Database;
using UnityEditor;
using UnityEngine;
using VComponent.InputSystem;
using Random = UnityEngine.Random;

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
		[SerializeField] private GameObject _waterPlanePrefab;

		[Header("Holders")]
		[SerializeField] private Transform _groundHolder;
		[SerializeField] private Transform _obstacleHolder;
		[SerializeField] private Transform _decorationHolder;

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

		public static string MapsPath => Path.Combine(Application.streamingAssetsPath, "Maps");

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
			if (Input.GetMouseButton(1))
			{
				DeletePreview();
			}

		}

		private void DeletePreview()
		{
			if (_currentTileController != null)
			{
				Destroy(_currentTileController.gameObject);
				_currentTileController = null;
			}

			if (_currentObstacleController != null)
			{
				Destroy(_currentObstacleController.gameObject);
				_currentObstacleController = null;
			}

			if (_currentDecorationController != null)
			{
				Destroy(_currentDecorationController.gameObject);
				_currentDecorationController = null;
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

			_currentTileController = GridObjectController.InstantiateFromTemplate(tileTemplate, _cellSize, _groundHolder) as TileController;
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

			_currentObstacleController = GridObjectController.InstantiateFromTemplate(obstacleTemplate, _cellSize, _groundHolder) as ObstacleController;
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

			_currentDecorationController = GridObjectController.InstantiateFromTemplate(decorationTemplate, _cellSize, _groundHolder) as DecorationController;
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
				
				var template = ScriptableObjectDatabase.GetTileTemplateByType(_currentTileController.TileType);
				var gridObjectController = GridObjectController.InstantiateAndAddToGridFromTemplate(template, chosenCell, _grid, _groundHolder);
				if (gridObjectController is TileController tileController)
				{
					chosenCell.AddTileToCell(tileController);
				}

				return;
			}

			if( _currentObstacleController != null)
			{
				if(chosenCell.ObstacleController != null)
				{
					Destroy(chosenCell.ObstacleController.gameObject);
				}
				var template = ScriptableObjectDatabase.GetObstacleTemplateByType(_currentObstacleController.ObstacleType);
				var gridObjectController = GridObjectController.InstantiateAndAddToGridFromTemplate(template, chosenCell, _grid, _groundHolder);
				if (gridObjectController is ObstacleController obstacleController)
				{
					chosenCell.AddObstacleToCell(obstacleController);
				}
			}
			else if (_currentDecorationController != null)
			{
				if (chosenCell.DecorationControllers != null && !_freePlacement && chosenCell.DetectDecorationOnCell(_currentDecorationController))
				{
					return;
				}
				
				var template = ScriptableObjectDatabase.GetDecorationTemplateByType(_currentDecorationController.DecorationType);
				var gridObjectController = GridObjectController.InstantiateAndAddToGridFromTemplate(template, chosenCell, _grid, _groundHolder);
				if (gridObjectController is DecorationController decorationController)
				{
					chosenCell.AddDecorationToCell(decorationController);
					
					if (_freePlacement)
					{
						decorationController.OverrideGridPosition(worldMousePosition);
					}
				}
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

			_grid = new Grid(_gridXValue, _gridYValue, _cellSize, _startPosition, false);
			_cellList = new List<Cell>();

			// Instantiate ground blocks
			for (int x = 0; x < _grid.GetWidth(); x++)
			{
				for (int z = 0; z < _grid.GetHeight(); z++)
				{
					_grid.TryGetCellByCoordinates(x, z, out var chosenCell);
					
					// TILES
					var template = ScriptableObjectDatabase.GetTileTemplateByType(TileType.GRASS);
					var gridObjectController = GridObjectController.InstantiateAndAddToGridFromTemplate(template, chosenCell, _grid, _groundHolder);
					if (gridObjectController is TileController tileController)
					{
						chosenCell.AddTileToCell(tileController);
					}
					
					_cellList.Add(chosenCell);
				}
			}
			
			AddWaterPlane();
		}
        
		private void AddWaterPlane()
		{
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
			
			InputManager.Instance.gameObject.SetActive(false);
			
			_grid = new Grid(_gridXValue, _gridYValue, _cellSize, _startPosition, false, serializedCellList.ToArray());
			_cellList = new List<Cell>();
			
			// Instantiate ground blocks
			for (int x = 0; x < _grid.GetWidth(); x++)
			{
				for (int z = 0; z < _grid.GetHeight(); z++)
				{
					_grid.TryGetCellByCoordinates(x, z, out var chosenCell);
					SerializedCell serializedCell = serializedCellList.Find(cell => cell.X == x && cell.Y == z);
					
					AddTileFromSerializedCell(serializedCell, chosenCell);

					if (serializedCell.ObstacleType != ObstacleType.NONE)
					{
						AddObstacleFromSerializedCell(serializedCell, chosenCell);
					}

					if (serializedCell.DecorationPositions != null && serializedCell.DecorationPositions.Count > 0)
					{
						AddDecorationsFromSerializedCell(serializedCell, chosenCell);
					}

					_cellList.Add(chosenCell);
				}
			}
		}
		
		private void AddTileFromSerializedCell(SerializedCell serializedCell, Cell chosenCell)
		{
			var template = ScriptableObjectDatabase.GetTileTemplateByType(serializedCell.TileType);
			var gridObjectController = GridObjectController.InstantiateAndAddToGridFromTemplate(template, chosenCell, _grid, _groundHolder);
			if (gridObjectController is TileController tileController)
			{
				chosenCell.AddTileToCell(tileController);
			}
		}
		
		private void AddObstacleFromSerializedCell(SerializedCell serializedCell, Cell chosenCell)
		{
			// Read obstacle rotation
			// TODO: Add auto getter in serialized class to do this operation
			float[] rotationArray = serializedCell.ObstacleRotation;
			Quaternion obstacleRotation = new Quaternion(rotationArray[0], rotationArray[1], rotationArray[2], rotationArray[3]);

			// Read obstacle scale
			// TODO: Add auto getter in serialized class to do this operation
			float[] scaleArray = serializedCell.ObstacleScale;
			Vector3 obstacleScale = new Vector3(scaleArray[0], scaleArray[1], scaleArray[2]);
						
			var template = ScriptableObjectDatabase.GetObstacleTemplateByType(serializedCell.ObstacleType);
			var gridObjectController = GridObjectController.InstantiateAndAddToGridFromTemplate(template, chosenCell, _grid, _groundHolder, obstacleRotation, obstacleScale);
			if (gridObjectController is ObstacleController obstacleController)
			{
				chosenCell.AddObstacleToCell(obstacleController);
			}
		}
		
		private void AddDecorationsFromSerializedCell(SerializedCell serializedCell, Cell chosenCell)
		{
			for (int i = 0; i < serializedCell.DecorationPositions.Count; i++)
			{
				// Decoration coordinates.
				// TODO: Add auto getter in serialized class to do this operation
				float[] positionArray = serializedCell.DecorationPositions[i];
				Vector3 decorationPosition = new Vector3(positionArray[0], positionArray[1], positionArray[2]);

				// Decoration rotation.
				// TODO: Add auto getter in serialized class to do this operation
				float[] rotationArray = serializedCell.DecorationRotations[i];
				Quaternion decorationRotation = new Quaternion(rotationArray[0], rotationArray[1], rotationArray[2], rotationArray[3]);

				// Decoration local scale.
				// TODO: Add auto getter in serialized class to do this operation
				float[] scaleArray = serializedCell.DecorationScales[i];
				Vector3 decorationScale = new Vector3(scaleArray[0], scaleArray[1], scaleArray[2]);

				var template = ScriptableObjectDatabase.GetDecorationTemplateByType(serializedCell.DecorationTypes[i]);
				var gridObjectController = GridObjectController.InstantiateAndAddToGridFromTemplate(template, chosenCell, _grid, _groundHolder, decorationRotation, decorationScale);
				if (gridObjectController is DecorationController decorationController)
				{
					decorationController.OverrideGridPosition(decorationPosition);
					chosenCell.AddDecorationToCell(decorationController);
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
#if UNITY_EDITOR
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
#endif
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

		public static bool TryLoadMapByName(string name, out SerializedCell[] serializedCells)
		{
			// Get all potential maps.
			var mapFileNames = GetAllMapFileNames();

			for (int i = 0; i < mapFileNames.Count; i++)
			{
				if (mapFileNames[i] == name)
				{
					if (TryLoadMapAt(i, out var cells))
					{
						serializedCells = cells;
						return true;
					}
				}
			}

			Debug.LogError($"No map with name: {name} found.");
			serializedCells = Array.Empty<SerializedCell>();
			return false;
		}

		public static bool TryLoadMapAt(int index, out SerializedCell[] serializedCells)
		{
			// Get all potential maps.
			var mapFileNames = GetAllMapFileNames();

			// Ensure index is within valid range
			if (mapFileNames.Count == 0 || index < 0 || index >= mapFileNames.Count)
			{
				Debug.LogError($"[TryLoadMapAt] Invalid index: {index}. Total maps: {mapFileNames.Count}.");
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