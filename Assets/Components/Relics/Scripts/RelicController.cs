using Components.Grid;
using Components.Interactions.Clickable;
using Components.Relics.Behavior;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Relics
{
	public class RelicController : MonoBehaviour, IClickable
	{
		[SerializeField] private Transform _3dViewHolder;
		[SerializeField] private Relic _relic;

		public static Action<RelicTemplate> OnRelicClicked;
		public Relic Relic => _relic;

		private bool _initialized;
		private GameObject _view;
		private RelicTemplate _template;

		public RelicTemplate Template => _template;

		private Cell _chosenCell;
		private int _radius;
		private int _mapWidth;
		private int _mapHeight;
		private Grid.Grid _grid;
		List<Cell> _zone;

		// ------------------------------------------------------------------------- INIT -------------------------------------------------------------------------
		public void InstantiatePreview(RelicTemplate relicTemplate, float scale)
		{
			_view = Instantiate(relicTemplate.GridView, _3dViewHolder);
			_relic = new Relic(relicTemplate, this);
			_view.transform.localScale = new Vector3(scale, _view.transform.localScale.y, scale);
			_template = relicTemplate;
		}

		public void RotatePreview(int angle)
		{
			_view.transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));

		}

		public void ConfirmPlacement()
		{
			_initialized = true;
			_relic.OnTick += Tick;

			
		}

		/// <summary>
		/// Gets the cells within a circular zone of a specified radius around an origin cell.
		/// </summary>
		/// <param name="originX">X-coordinate of the origin cell.</param>
		/// <param name="originY">Y-coordinate of the origin cell.</param>
		/// <param name="radius">The radius of the zone.</param>
		/// <returns>A list of Vector2Int positions representing the cells within the zone.</returns>
		public List<Cell> GetZone(Cell origineCell, int radius, int mapWidth, int mapHeight, Grid.Grid grid)
		{
			List<Cell> zoneCells = new List<Cell>();

			for (int x = Mathf.Max(0, origineCell.X - radius); x <= Mathf.Min(mapWidth - 1, origineCell.X + radius); x++)
			{
				for (int y = Mathf.Max(0, origineCell.Y - radius); y <= Mathf.Min(mapHeight - 1, origineCell.Y + radius); y++)
				{
					float distance = Mathf.Sqrt(Mathf.Pow(x - origineCell.X, 2) + Mathf.Pow(y - origineCell.Y, 2));
					if (distance <= radius)
					{
						if (!grid.TryGetCellByCoordinates(x, y, out Cell overlapCell))
						{
							continue;
						}
						zoneCells.Add(overlapCell);
					}
				}
			}

			return zoneCells;
		}

		/// <summary>
		/// Visualizes the zone in the Unity Editor using Gizmos.
		/// </summary>
		/// <param name="originX">X-coordinate of the origin cell.</param>
		/// <param name="originY">Y-coordinate of the origin cell.</param>
		/// <param name="radius">The radius of the zone.</param>
		public void DrawZoneGizmos(Cell chosenCell, int radius, int mapWidth, int mapHeight, Grid.Grid grid)
		{
			_chosenCell = chosenCell;
			_radius = radius;
			_mapWidth = mapWidth;
			_mapHeight = mapHeight;
			_grid = grid;
		}


		private void OnDrawGizmos()
		{
			if(_chosenCell == null)
			{
				return;
			}

			_zone = GetZone(_chosenCell, _radius, _mapWidth, _mapHeight, _grid);
			Gizmos.color = Color.red;

			foreach (Cell cell in _zone)
			{
				Vector3 cellPosition = new Vector3(cell.X + cell.Size / 2, 0 , cell.Y + cell.Size / 2);
				Gizmos.DrawCube(cellPosition, new Vector3(1,0.1f,1));; // Draw a cube for each cell in the zone
			}
		}

		private void OnDestroy()
		{
			if (!_initialized)
			{
				return;
			}

			_relic.OnTick -= Tick;

		}

		public void Clicked()
		{
			if (!_initialized)
			{
				return;
			}

			OnRelicClicked?.Invoke(_template);
		}

		private void Tick()
		{
			
		}
	}

}
