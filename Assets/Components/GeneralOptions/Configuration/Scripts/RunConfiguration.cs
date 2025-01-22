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
	[SerializeField] private int _goldAmountPerRound = 5;

	[Tooltip("For every gold interest value give the GoldInterestAmountPerRound as bonus gold")]
	[SerializeField] private int _goldInterestValue = 5;
	[SerializeField] private int _goldInterestAmountPerRound = 1;
	
	public List<RunStateConfiguration> RunStateList => _runStateList;
	public int GoldAmountPerRound => _goldAmountPerRound;
	public int GoldInterestValue => _goldInterestValue;
	public int GoldInterestAmountPerRound => _goldInterestAmountPerRound;

	//-------------------------------------------------------------------------------------------------- METHODS -------------------------------------------------------------------------------------------------------------------
	public List<IngredientTemplate> GetPossibleIngredients(int stateNumber, List<IngredientTemplate> baseIngredients)
	{
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
		return IngredientTransformation.FindMakeableIngredients(allRecipes, baseIngredients).ToList();
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


