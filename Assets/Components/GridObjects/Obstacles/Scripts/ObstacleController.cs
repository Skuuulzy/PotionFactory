using Components.Grid.Tile;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Grid.Obstacle
{
	public class ObstacleController : GridObjectController
	{

		[SerializeField] private ObstacleType _obstacleType;
		public ObstacleType ObstacleType => _obstacleType;

		// ------------------------------------------------------------------------- INIT -------------------------------------------------------------------------

		public void SetObstacleType(ObstacleType obstacleType)
		{
			_obstacleType = obstacleType;
		}
	}

	public enum ObstacleType
	{
		NONE,
		TREE1,
		TREE2,
		BUSH1,
		BUSH2
	}
}
