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
		TreeE04,
		Barrel,
		Barrel_Bananas,
		Barrel_Bomb,
		Barrel_Coconuts,
		Barrel_Fabrics,
		Barrel_Rhum,
		Barrel_Shells,
		Barrel_Stones,
		Bridge_Plank_01,
		Bridge_Plank_02,
		Cart,
		Dead_Branch,
		Dead_Large_Trunk,
		Fence_Ranch_01,
		Fence_Ranch_02,
		Fence_Ranch_03,
		Fence_Ranch_04,
		Fence_Wooden_01,
		Fence_Wooden_02,
		Fence_Wooden_03,
		Ground_Plateforme,
		Lamp_Pole,
		Large_Boxe,
		Small_Boxe,
		Pilard_01,
		Pilard_02,
		Pilard_03,
		Pilard_04,
		Small_Boat,
		Table,
		Top_Pilard,
		Top_Wall,
		Wall_01,
		Wall_02,
		Well,
		Dead_Trunk,

	}
}
