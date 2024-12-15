using System;
using UnityEngine;

namespace Components.Ingredients
{
	[CreateAssetMenu(fileName = "New Ingredient Template", menuName = "Ingredients/Template")]
	public class IngredientTemplate : ScriptableObject
	{
		[SerializeField] private string _name;
		[SerializeField] private int _price;
		[SerializeField] private int _nbOfTransfo;
		[SerializeField] private bool _isLiquid;
		[SerializeField] private float _executionTimeModifier;
		
		[SerializeField] private GameObject _3dView;
		[SerializeField] private Sprite _icon;

		[Header("Debug")]
		[SerializeField] private GameObject _debug3DView;

		public int Price => _price;
		public string Name => _name;
		public int NumberOfTransformation => _nbOfTransfo;
		public bool IsLiquid => _isLiquid;
		public float ExecutionTimeModifier => _executionTimeModifier;
		public GameObject View => _3dView;
		public Sprite Icon => _icon;

		public void CreateFromCSV(string ingredientName, int price, int nbTransfor, bool isLiquid, float executionTimeModifier)
		{
			_name = ingredientName;
			_price = price;
			_nbOfTransfo = nbTransfor;
			_isLiquid = isLiquid;
			_executionTimeModifier = executionTimeModifier;
		}
	}
}