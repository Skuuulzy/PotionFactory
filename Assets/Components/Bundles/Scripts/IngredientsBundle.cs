using Components.Ingredients;
using Components.Machines;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Bundle
{
	[CreateAssetMenu(fileName = "New Ingredients Bundle", menuName = "Bundle/Ingredients Bundle")]
	public class IngredientsBundle : Bundle
	{
		[SerializeField] private List<IngredientTemplate> _ingredientsTemplateList;
		[SerializeField] private List<MachineTemplate> _machinesTemplateList;


		public List<IngredientTemplate> IngredientsTemplatesList => _ingredientsTemplateList;
		public List<MachineTemplate> MachinesTemplateList => _machinesTemplateList;
	}
}
