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
	[Header("")]
	[SerializeField] private List<RunStateConfiguration> _runStateList;

	[Header("GuildTicket")]
	[SerializeField] private int _guildTicketAmountPerRound = 5;
	[Tooltip("For every gold interest value give the GoldInterestAmountPerRound as bonus gold")]
	[SerializeField] private int _guildTicketInterestValue = 5;
	[SerializeField] private int _guildTicketInterestAmountPerRound = 1;

	[Header("StateTime")]
	[SerializeField] private int _planningFactoryStateTime = 120;
	[SerializeField] private int _resolutionFactoryStateTime = 180;

	public List<RunStateConfiguration> RunStateList => _runStateList;
	public int GuildTicketAmountPerRound => _guildTicketAmountPerRound;
	public int GuildTicketInterestValue => _guildTicketInterestValue;
	public int GuildTicketInterestAmountPerRound => _guildTicketInterestAmountPerRound;
	public int PlanningFactoryStateTime => _planningFactoryStateTime;
	public int ResolutionFactoryStateTime => _resolutionFactoryStateTime;

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

	public int GetStateTime(int stateNumber)
	{
		if(_runStateList[stateNumber].CustomTime != 0)
		{
			return _runStateList[stateNumber].CustomTime;
		}
		else
		{
			return ResolutionFactoryStateTime;
		}
	}

	//-------------------------------------------------------------------------------------------------- STRUCTS -------------------------------------------------------------------------------------------------------------------
	[Serializable]
	public struct RunStateConfiguration
	{
		public int StateNumber;
		public int NumberOfTransformationAsked;
		public TypeOfNumberTransformation TypeOfNumberTransformation;
		public int MoneyObjective;
		public int CustomTime;
	}

	[Serializable]
	public enum TypeOfNumberTransformation
	{
		Inferior,
		Equal,
		Superior
	}

}


