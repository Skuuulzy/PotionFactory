using UnityEngine;

namespace Components.Grid.Obstacle
{
	[CreateAssetMenu(fileName = "New Obstacle Template", menuName = "Grid/Obstacle Template")]
	public class ObstacleTemplate : GridObjectTemplate
    {
		[SerializeField] private ObstacleType _obstacleType;
		[SerializeField] private ObstacleCategory _category;

		public ObstacleType ObstacleType => _obstacleType;
		public ObstacleCategory Category => _category;
	}

	public enum ObstacleCategory
	{
		Trees,
		Bushs,
		Rocks,
		Walls,
		Props
	}
}
