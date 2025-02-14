using Components.Bundle;
using Components.Economy;
using Components.Ingredients;
using Components.Inventory;
using Components.Machines;
using Components.Machines.Behaviors;
using Components.Shop.ShopItems;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VComponent.Tools.SceneLoader;

public class UIGameOverController : MonoBehaviour
{
	[SerializeField] private GameObject _view;
	[SerializeField] private Toggle _showViewToggle;

    [Header("Defeat")]
    [SerializeField] private TextMeshProUGUI _defeatExplicationText;

	[SerializeField] private UIBundleView _bundleViewPrefab;

	[Header("StartingBundle")]
    [SerializeField] private Transform _startingBundleTransform;

	[Header("OtherIngredients")]
	[SerializeField] private Transform _otherIngredientTransform;

	[Header("MachinesPurchased")]
	[SerializeField] private Transform _machinesPurchasedTransform;

	[Header("RecipesCreated")]
	[SerializeField] private Transform _recipesSoldTransform;

	private IngredientsBundle _startingIngredientBundle;
	private List<IngredientsBundle> _otherIngredientsList = new List<IngredientsBundle>();
	private Dictionary<MachineTemplate, int> _machinesPossedByPlayer = new Dictionary<MachineTemplate, int>();
	private Dictionary<IngredientTemplate, int> _recipesSold = new Dictionary<IngredientTemplate, int>();

	private void Start()
	{
		EconomyController.OnGameOver += HandleGameOver;
		BundleChoiceGenerator.OnBundleChoiceConfirm += HandleBundleChoice;
		MarchandMachineBehaviour.OnIngredientSold += HandleIngredientSold;
		UIMachineShopItemViewController.OnMachineBuyed += HandleMachineAdded;
	}

	private void OnDestroy()
	{
		EconomyController.OnGameOver -= HandleGameOver;

		BundleChoiceGenerator.OnBundleChoiceConfirm -= HandleBundleChoice;
		MarchandMachineBehaviour.OnIngredientSold -= HandleIngredientSold;
		UIMachineShopItemViewController.OnMachineBuyed -= HandleMachineAdded;

	}

	public void HandleGameOver(int playerScore, int scoreObjective, int day)
	{
		EconomyController.OnGameOver -= HandleGameOver;

		_view.SetActive(true);
		_showViewToggle.gameObject.SetActive(true);
		_defeatExplicationText.text = $"Defeat at Day {day}. You needed to do {scoreObjective} golds and you did {playerScore}.";

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
		_view.SetActive(value);
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
		foreach(IngredientsBundle bundle in _otherIngredientsList)
		{
			foreach (var ingredientsTemplate in bundle.IngredientsTemplatesList)
			{
				UIBundleView bundleView = Instantiate(_bundleViewPrefab, _otherIngredientTransform);
				bundleView.SetInfos(ingredientsTemplate.Icon, ingredientsTemplate.Name);
			}
		}
	}

	private void SetUpMachinesPurchased()
	{
		foreach(var kvp in _machinesPossedByPlayer)
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
		else
		{
			_otherIngredientsList.Add(bundle);
		}
	}

	private void HandleIngredientSold(IngredientTemplate template)
	{
		_recipesSold.TryAdd(template,0);
		_recipesSold[template]++;
	}

	private void HandleMachineAdded(MachineTemplate template)
	{
		_machinesPossedByPlayer.TryAdd(template, 0);
		_machinesPossedByPlayer[template]++;
	}


}
