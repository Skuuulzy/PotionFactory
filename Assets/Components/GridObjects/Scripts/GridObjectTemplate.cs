using UnityEngine;

namespace Components.Grid
{
	public class GridObjectTemplate : ScriptableObject
	{
		[Header("Definition")]
		[SerializeField] protected string _name;
		[SerializeField] protected GameObject _gridView;
		[SerializeField] protected Sprite _uiView;
		[SerializeField] protected GridObjectController _gridObjectControllerPrefab;

		[Header("Grid placement behaviour")] 
		[SerializeField] [Tooltip("Does the object can overwrite machine marked as Overwritable when placed on the grid")] 
		private bool _canOverwrite;
		[SerializeField] [Tooltip("Does the object can overwrite machine marked as Overwritable when placed on the grid")] 
		private bool _isOverwritable;
		[SerializeField] private bool _canRetrieve = true;
		[SerializeField] private bool _canMove = true;
		[SerializeField] private bool _canConfigure = true;

		public string Name => _name;
		public GameObject GridView => _gridView;
		public Sprite UIView => _uiView;
		public GridObjectController GridObjectControllerPrefab => _gridObjectControllerPrefab;

		public bool CanOverwrite => _canOverwrite;
		public bool IsOverwritable => _isOverwritable;
		public bool CanRetrieve => _canRetrieve;
		public bool CanMove => _canMove;
		public bool CanConfigure => _canConfigure;
	}
}