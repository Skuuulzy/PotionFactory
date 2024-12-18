using UnityEngine;

namespace Components.Grid.Obstacle
{
	[CreateAssetMenu(fileName = "New Obstacle Template", menuName = "Grid/Obstacle Template")]
	public class ObstacleTemplate : GridObjectTemplate
    {
		[SerializeField] private ObstacleType _obstacleType;

		public ObstacleType ObstacleType => _obstacleType;
	}
}
