using UnityEngine;

namespace Components.Grid
{
	public class GridObjectTemplate : ScriptableObject
	{
		[Header("Definition")]
		[SerializeField] protected string _name;
		[SerializeField] protected GameObject _gridView;
		[SerializeField] protected Sprite _uiView;
		[SerializeField] protected GridObjectController _gridObject;

		public string Name => _name;
		public GameObject GridView => _gridView;
		public Sprite UIView => _uiView;
		public GridObjectController GridObject => _gridObject;
	}
}