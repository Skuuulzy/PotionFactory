using System.Collections.Generic;
using UnityEngine;

namespace Components.Items
{
	[CreateAssetMenu(fileName = "New Item Template", menuName = "Items/Item Template")]
	public class ItemTemplate : ScriptableObject
	{
		[SerializeField] private string _name;
		[SerializeField] private int _price;
		[SerializeField] private bool _isLiquid;
		[SerializeField] private float _executionTimeModifier;

		[SerializeField] private List<Resource> _resources;
		[SerializeField] private List<ItemState> _types;
		[SerializeField] private GameObject _3dView;

		[Header("Debug")]
		[SerializeField] private GameObject _debug3DView;

		public int Price => _price;
		public string Name => _name;
		public bool IsLiquid => _isLiquid;
		public float ExecutionTimeModifier => _executionTimeModifier;
		public List<Resource> Resources => _resources;
		public List<ItemState> Types => _types;
		public GameObject View => _3dView;

		public void CreateItemWithoutRecipe(List<ItemTemplate> itemsTemplateUsedToCreate)
		{
			foreach(ItemTemplate itemTemplate in itemsTemplateUsedToCreate)
			{
				_resources.AddRange(itemTemplate.Resources);
				_types.AddRange(itemTemplate.Types);
			}

			_3dView = _debug3DView;
		}
	}
}