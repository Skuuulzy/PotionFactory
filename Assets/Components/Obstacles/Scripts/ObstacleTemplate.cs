using UnityEngine;

namespace Components.Grid.Obstacle
{
	[CreateAssetMenu(fileName = "New Obstacle Template", menuName = "Grid/Obstacle Template")]
	public class ObstacleTemplate : ScriptableObject
    {
		[Header("Definition")]
		[SerializeField] private string _name;
		[SerializeField] private ObstacleType _obstacleType;

		[SerializeField] private GameObject _gridView;
		[SerializeField] private Sprite _uiView;

		public string Name => _name;
		public ObstacleType ObstacleType => _obstacleType;
		public GameObject GridView => _gridView;
		public Sprite UIView => _uiView;
	}
}
