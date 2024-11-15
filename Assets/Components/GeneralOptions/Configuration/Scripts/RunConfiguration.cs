using Components.Ingredients;
using Components.Recipes;
using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Run Configuration", menuName = "Configuration/RunConfiguration")]
public class RunConfiguration : ScriptableObject
{
	[SerializeField] private List<RunStateConfiguration> _runStateList;
	public List<RunStateConfiguration> RunStateList => _runStateList;

	//-------------------------------------------------------------------------------------------------- METHODS -------------------------------------------------------------------------------------------------------------------
	public List<IngredientTemplate> GetPossibleIngredients(int stateNumber, List<IngredientTemplate> baseIngredients)
	{
		List<IngredientTemplate> finalListOfIngredient = new List<IngredientTemplate>();
		RunStateConfiguration runStateConfiguration = _runStateList.Find(configuration => configuration.StateNumber == stateNumber);

		//Get all items
		List<RecipeTemplate> allRecipes = ScriptableObjectDatabase.GetAllScriptableObjectOfType<RecipeTemplate>().ToList();

		//First filter : Based of number of transformation asked
		switch (runStateConfiguration.TypeOfNumberTransformation)
		{
			case TypeOfNumberTransformation.Inferior:
				allRecipes = allRecipes.Except(allRecipes.Where(x => x.OutIngredient.NumberOfTransformation > runStateConfiguration.NumberOfTransformationAsked)).ToList();
			break;

			case TypeOfNumberTransformation.Equal:
				allRecipes = allRecipes.Except(allRecipes.Where(x => x.OutIngredient.NumberOfTransformation != runStateConfiguration.NumberOfTransformationAsked)).ToList();
				break;

			case TypeOfNumberTransformation.Superior:
				allRecipes = allRecipes.Except(allRecipes.Where(x => x.OutIngredient.NumberOfTransformation < runStateConfiguration.NumberOfTransformationAsked)).ToList();
				break;
		}


		//Second filter : Get only ingredient that can be done by basic ingredient
		finalListOfIngredient = IngredientTransformation.FindMakeableIngredients(allRecipes, baseIngredients).ToList();

		return finalListOfIngredient;
	}

	//-------------------------------------------------------------------------------------------------- STRUCTS -------------------------------------------------------------------------------------------------------------------
	[Serializable]
	public struct RunStateConfiguration
	{
		public int StateNumber;
		public int NumberOfTransformationAsked;
		public TypeOfNumberTransformation TypeOfNumberTransformation;
		public int MoneyObjective;
	}

	[Serializable]
	public enum TypeOfNumberTransformation
	{
		Inferior,
		Equal,
		Superior
	}

}


