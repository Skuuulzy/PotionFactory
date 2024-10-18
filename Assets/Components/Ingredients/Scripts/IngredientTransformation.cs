using Components.Ingredients;
using Components.Recipes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class IngredientTransformation
{

	// Dictionary to hold the transformation relationships
	private static Dictionary<IngredientTemplate, List<IngredientTemplate>> _ingredientDependencies = new Dictionary<IngredientTemplate, List<IngredientTemplate>>();


	/// <summary>
	/// Dynamically builds the ingredient dependencies based on the recipe templates.
	/// </summary>
	public static List<IngredientTemplate> FindMakeableIngredients(List<RecipeTemplate> recipeTemplates, List<IngredientTemplate> baseIngredients)
	{
		foreach (RecipeTemplate recipe in recipeTemplates)
		{
			IngredientTemplate outIngredient = recipe.OutIngredient;
			Dictionary<IngredientTemplate, int> requiredIngredients = recipe.Ingredients;

			// Convert required ingredients into their names
			List<IngredientTemplate> ingredients = new List<IngredientTemplate>();
			foreach (var ingredientEntry in requiredIngredients)
			{
				ingredients.Add(ingredientEntry.Key);
			}

			// Add the dependency relationship to the dictionary
			_ingredientDependencies[outIngredient] = ingredients;
		}

		HashSet<IngredientTemplate> makeableIngredients = new HashSet<IngredientTemplate>(baseIngredients);

		// Traverse through all ingredients
		foreach (IngredientTemplate ingredient in _ingredientDependencies.Keys)
		{
			if (CanMakeIngredient(ingredient, makeableIngredients))
			{
				makeableIngredients.Add(ingredient);
			}
		}

		return new List<IngredientTemplate>(makeableIngredients);
	}


	/// <summary>
	/// Determines if an ingredient can be made using available ingredients.
	/// </summary>
	private static bool CanMakeIngredient(IngredientTemplate ingredient, HashSet<IngredientTemplate> availableIngredients)
	{
		if (!_ingredientDependencies.ContainsKey(ingredient))
			return false;

		foreach (IngredientTemplate dependency in _ingredientDependencies[ingredient])
		{
			if (!availableIngredients.Contains(dependency))
				return false;
		}

		return true;
	}
}
