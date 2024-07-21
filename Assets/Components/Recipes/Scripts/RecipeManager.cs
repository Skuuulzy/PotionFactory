using Components.Items;
using Components.Machines;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Recipes
{
	public class RecipeManager : ScriptableObject
	{
		[SerializeField] private List<RecipeTemplate> _recipeTemplatesList;
		[SerializeField] private RecipeTemplate _unknownItemRecipe;
		
		public bool TryFindRecipe(MachineTemplate machineTemplate, List<IngredientTemplate> inputsItems, out RecipeTemplate recipe)
		{
			foreach (RecipeTemplate recipeTemplate in _recipeTemplatesList)
			{
				if (recipeTemplate.Machine != machineTemplate) 
					continue;
				
				foreach (IngredientTemplate itemTemplate in inputsItems)
				{
					if (!IsItemMatch(itemTemplate, recipeTemplate.ItemsUsedInRecipe.ToDictionary()))
					{
						continue;
					}
					
					recipe = recipeTemplate;
					return true;
				}
			}

			//We don't find any recipes SO we return an unknown item recipe.
			recipe = _unknownItemRecipe;
			return false;
		}

		private bool IsItemMatch(IngredientTemplate ingredient, Dictionary<IngredientTemplate, int> item2)
		{
			return item2.ContainsKey(ingredient);
		}
	}
}