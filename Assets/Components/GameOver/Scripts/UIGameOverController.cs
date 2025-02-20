using Components.Bundle;
using Components.Economy;
using Components.Ingredients;
using Components.Machines;
using Components.Machines.Behaviors;
using Components.Shop.ShopItems;
using SoWorkflow.SharedValues;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VComponent.Tools.SceneLoader;

public class UIGameOverController : MonoBehaviour
{
	[SerializeField] private GameObject _view;
	[SerializeField] private List<GameObject> _objectsToHide;
	[SerializeField] private Toggle _showViewToggle;

    [Header("Defeat")]
    [SerializeField] private TextMeshProUGUI _defeatExplicationText;
	[SerializeField] private UIBundleView _bundleViewPrefab;

	[Header("Holders")]
    [SerializeField] private Transform _startingBundleTransform;
	[SerializeField] private Transform _otherIngredientTransform;
	[SerializeField] private Transform _machinesPurchasedTransform;
	[SerializeField] private Transform _recipesSoldTransform;

	[Header("SharedValues")]
	[SerializeField] private SOSharedInt _dayIndex; 
	[SerializeField] private SOSharedInt _playerScore; 
	[SerializeField] private SOSharedInt _playerObjective; 

	private IngredientsBundle _startingIngredientBundle;
	private readonly List<IngredientTemplate> _otherIngredientsList = new();
	private readonly Dictionary<MachineTemplate, int> _machinesPossessedByPlayer = new();
	private readonly Dictionary<IngredientTemplate, int> _recipesSold = new();

	private void Start()
	{
        EndGameState.OnEndGameStateStarted += HandleGameOver;
        BundleChoiceGenerator.OnBundleChoiceConfirm += HandleBundleChoice;
		UIIngredientShopItemViewController.OnIngredientBuyed += HandleIngredientBuyed;
		MarchandMachineBehaviour.OnIngredientSold += HandleIngredientSold;
		UIMachineShopItemViewController.OnMachineBuyed += HandleMachineAdded;
	}

	private void OnDestroy()
	{
		EndGameState.OnEndGameStateStarted -= HandleGameOver;
        BundleChoiceGenerator.OnBundleChoiceConfirm -= HandleBundleChoice;
        UIIngredientShopItemViewController.OnIngredientBuyed -= HandleIngredientBuyed;
        MarchandMachineBehaviour.OnIngredientSold -= HandleIngredientSold;
		UIMachineShopItemViewController.OnMachineBuyed -= HandleMachineAdded;

	}



    private void HandleGameOver(EndGameState state)
	{
        EndGameState.OnEndGameStateStarted -= HandleGameOver;

        _view.SetActive(true);
		_showViewToggle.gameObject.SetActive(true);
		_defeatExplicationText.text = $"Defeated at Day {_dayIndex.Value}.\nYour goal was to reach {_playerObjective.Value} gold, and you’ve achieved only {_playerScore.Value}...";

		SetUpStartingBundle();
		SetUpOtherIngredients();
		SetUpMachinesPurchased();
		SetUpRecipesSold();
	}

	public void ReturnToMainMenu()
	{
		SceneLoader.LoadMainMenu();
	}

	public void DisplayGameOver(bool value)
	{
		for (int i = 0; i < _objectsToHide.Count; i++)
		{
			_objectsToHide[i].SetActive(value);
		}
	}

	//------------------------------------------------------------ SetUp ------------------------------------------------------------------------------------

	private void SetUpStartingBundle()
	{
		foreach (var machineTemplate in _startingIngredientBundle.MachinesTemplateList)
		{
			UIBundleView bundleView = Instantiate(_bundleViewPrefab, _startingBundleTransform);
			bundleView.SetInfos(machineTemplate.UIView, machineTemplate.Name);
		}

		foreach (var ingredientsTemplate in _startingIngredientBundle.IngredientsTemplatesList)
		{
			UIBundleView bundleView = Instantiate(_bundleViewPrefab, _startingBundleTransform);
			bundleView.SetInfos(ingredientsTemplate.Icon, ingredientsTemplate.Name);
		}
	}

	private void SetUpOtherIngredients()
	{
		foreach(IngredientTemplate ingredient in _otherIngredientsList)
		{
            UIBundleView bundleView = Instantiate(_bundleViewPrefab, _otherIngredientTransform);
            bundleView.SetInfos(ingredient.Icon, ingredient.Name);
		}
	}

	private void SetUpMachinesPurchased()
	{
		foreach(var kvp in _machinesPossessedByPlayer)
		{
			UIBundleView bundleView = Instantiate(_bundleViewPrefab, _machinesPurchasedTransform);
			bundleView.SetInfos(kvp.Key.UIView, $"{kvp.Key.Name} x{kvp.Value}");
		}
	}

	private void SetUpRecipesSold()
	{
		foreach (var kvp in _recipesSold)
		{
			UIBundleView bundleView = Instantiate(_bundleViewPrefab, _recipesSoldTransform);
			bundleView.SetInfos(kvp.Key.Icon, $"{kvp.Key.Name} x{kvp.Value}");
		}
	}

	//------------------------------------------------------------ HANDLE ------------------------------------------------------------------------------------
	private void HandleBundleChoice(IngredientsBundle bundle, bool isStartingChoice)
	{
		if (isStartingChoice)
		{
			_startingIngredientBundle = bundle;
		}
	}
    private void HandleIngredientBuyed(IngredientTemplate template)
    {
		_otherIngredientsList.Add(template);
    }

    private void HandleIngredientSold(IngredientTemplate template)
	{
		_recipesSold.TryAdd(template,0);
		_recipesSold[template]++;
	}

	private void HandleMachineAdded(MachineTemplate template)
	{
		_machinesPossessedByPlayer.TryAdd(template, 0);
		_machinesPossessedByPlayer[template]++;
	}
}