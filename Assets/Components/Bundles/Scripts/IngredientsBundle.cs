using Components.Ingredients;
using Components.Machines;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Bundle
{
	[CreateAssetMenu(fileName = "New Ingredients Bundle", menuName = "Component/Bundle/Ingredients Bundle")]
	public class IngredientsBundle : Bundle
	{
		[SerializeField] private List<IngredientTemplate> _ingredientsTemplateList;
		[SerializeField] private List<MachineTemplate> _machinesTemplateList;
		[SerializeField] private bool _isStartingBundle;
		[SerializeField] private IngredientRarity _rarity;

		public List<IngredientTemplate> IngredientsTemplatesList => _ingredientsTemplateList;
		public List<MachineTemplate> MachinesTemplateList => _machinesTemplateList;
		public bool IsStartingGameBundle => _isStartingBundle;
		public IngredientRarity Rarity => _rarity;
	}

	public enum IngredientRarity
	{
		Rare
	}
}
