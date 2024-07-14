using Components.Items;
using Components.Machines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Recipes
{
	public class RecipeManager : ScriptableObject
	{
		[SerializeField] private List<RecipeTemplate> _recipeTemplatesList;
		public ItemTemplate FindTheRecipe(MachineTemplate machineTemplate, List<ItemTemplate> items)
		{
			foreach (RecipeTemplate recipeTemplate in _recipeTemplatesList)
			{
				if (recipeTemplate.Machine == machineTemplate)
				{
					bool found = false;
					foreach (ItemTemplate itemTemplate in items)
					{
						found = IsItemMatch(itemTemplate, recipeTemplate.ItemsUsedInRecipe.ToDictionary());
						if (found == false)
						{
							break;
						}
					}

					if (found == true)
					{
						return recipeTemplate.OutItemTemplate;
					}
				}
			}

			//We don't find any recipes so we create a new items  with a debug 3D representation
			return new ItemTemplate(items);
		}

		private bool IsItemMatch(ItemTemplate item, Dictionary<ItemTemplate, int> item2)
		{
			if (item2.ContainsKey(item) == false)
			{
				return false;
			}
			return true;
		}
	}
}

