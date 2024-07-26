using Components.Grid.Tile;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Grid.Obstacle
{
	public class ObstacleController : MonoBehaviour
	{
		[SerializeField] private Transform _3dViewHolder;
		[SerializeField] private ObstacleType _obstacleType;
		[SerializeField] private Cell _cell;

		private GameObject _view;

		public Transform View => _3dViewHolder;
		public ObstacleType ObstacleType => _obstacleType;
		public Cell Cell => _cell;

		// ------------------------------------------------------------------------- INIT -------------------------------------------------------------------------
		public void InstantiatePreview(ObstacleTemplate obstacleTemplate, float scale)
		{
			_view = Instantiate(obstacleTemplate.GridView, _3dViewHolder);

			_view.transform.localScale = new Vector3(scale, scale, scale);
		}

		public void SetCell(Cell cell)
		{
			_cell = cell;
		}

		public void SetObstacleType(ObstacleType obstacleType)
		{
			_obstacleType = obstacleType;
		}
	}

	public enum ObstacleType
	{
		TREE1,
		TREE2,
		BUSH1,
		BUSH2
	}
}
