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
		BushA01,
		BushA02,
		BushA03,
		BushA04,
		BushB01,
		BushB02,
		BushB03,
		BushC01,
		BushC02,
		BushC03,
		TreeA01,
		TreeA02,
		TreeA03,
		TreeA04,
		TreeB01,
		TreeB02,
		TreeB03,
		TreeB04,
		TreeC01,
		TreeC02,
		TreeC03,
		TreeC04,
		TreeD01,
		TreeD02,
		TreeD03,
		TreeD04,
		TreeE01,
		TreeE02,
		TreeE03,
		TreeE04
	}
}
