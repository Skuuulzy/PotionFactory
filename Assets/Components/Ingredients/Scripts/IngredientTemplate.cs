using System.Collections.Generic;
using UnityEngine;

namespace Components.Items
{
	[CreateAssetMenu(fileName = "New Ingredient Template", menuName = "Ingredients/Template")]
	public class IngredientTemplate : ScriptableObject
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

		public void CreateFromCSV(string ingredientName, int price, bool isLiquid, float executionTimeModifier)
		{
			_name = ingredientName;
			_price = price;
			_isLiquid = isLiquid;
			_executionTimeModifier = executionTimeModifier;
		}

		public void CreateItemWithoutRecipe(List<IngredientTemplate> itemsTemplateUsedToCreate)
		{
			foreach(IngredientTemplate itemTemplate in itemsTemplateUsedToCreate)
			{
				_resources.AddRange(itemTemplate.Resources);
				_types.AddRange(itemTemplate.Types);
			}

			_3dView = _debug3DView;
		}
	}
}