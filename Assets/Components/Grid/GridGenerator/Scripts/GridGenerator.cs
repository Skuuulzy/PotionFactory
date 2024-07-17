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

		// Grid
		private Grid _grid;
		private readonly Dictionary<Cell, GameObject> _instancedObjects = new();

		private UnityEngine.Camera _camera;

		// ------------------------------------------------------------------------- MONO -------------------------------------------------------------------------
		private void Start()
		{
			_camera = UnityEngine.Camera.main;
			GenerateGrid();
		}

		public void GenerateGrid()
		{
			_grid = new Grid(_gridXValue, _gridYValue, _cellSize, _startPosition, _groundHolder, false);

		}
	}
}
