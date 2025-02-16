using Components.Bundle;
using Components.Order;
using Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Components.Bundle
{
	public class BundleChoiceGenerator : MonoBehaviour
	{
		[SerializeField] private GameObject _bundleChoiceGameObject;
		[SerializeField] private Button _confirmButton;
		[SerializeField] private List<UIBundleChoiceController> _uiBundleChoiceList;

		[Header("Helps")]
		[SerializeField] private OrderDialogueController _orderDialogueController;


		private bool _isFirstGameChoice;

		private List<IngredientsBundle> _startingGameIngredientsBundles;
		private List<IngredientsBundle> _startingRoundIngredientsBundles;

		private IngredientsBundle _currentBundleSelected;

		//Need to change this poor bool 
		public static Action<IngredientsBundle, bool> OnBundleChoiceConfirm;
		// Start is called before the first frame update
		void Start()
		{
			BundleChoiceState.OnBundleStateStarted += Init;
		}

		private void OnDestroy()
		{
			BundleChoiceState.OnBundleStateStarted -= Init;
		}

		private void Init(BundleChoiceState state)
		{
			_bundleChoiceGameObject.SetActive(true);
			_orderDialogueController.gameObject.SetActive(true);

			//First map generation
			if (state.StateIndex == 1)
			{
				_startingGameIngredientsBundles = ScriptableObjectDatabase.GetAllScriptableObjectOfType<IngredientsBundle>().Where(bundle => bundle.IsStartingGameBundle).ToList();
				_startingRoundIngredientsBundles = ScriptableObjectDatabase.GetAllScriptableObjectOfType<IngredientsBundle>().Where(bundle => !bundle.IsStartingGameBundle).ToList();
				_isFirstGameChoice = true;
				_orderDialogueController.SetText("Choose your starting bundle, young apprentice!");
			}
			else
			{
				_isFirstGameChoice = false;
				_orderDialogueController.SetText("Choose a new ingredient!");
			}

			RegenerateBundleChoice();
			UIBundleChoiceController.OnBundleSelected += SelectCurrentBundle;
		}

		private void RegenerateBundleChoice()
		{
			_currentBundleSelected = null;
			_confirmButton.interactable = false;
			var startingRoundIngredientsBundles = new List<IngredientsBundle>();

			if (_isFirstGameChoice)
			{
				startingRoundIngredientsBundles = _startingGameIngredientsBundles.OrderBy(_ => UnityEngine.Random.value).ToList();
			}
			else
			{
				startingRoundIngredientsBundles = _startingRoundIngredientsBundles.OrderBy(_ => UnityEngine.Random.value).ToList();
			}
			
			for (int i = 0; i < _uiBundleChoiceList.Count; i++)
			{
				_uiBundleChoiceList[i].SetInfos(startingRoundIngredientsBundles[i]);
			}
		}

		private void SelectCurrentBundle(IngredientsBundle bundle)
		{
			_currentBundleSelected = bundle;
			_confirmButton.interactable = true;
			foreach(var bundleChoice in _uiBundleChoiceList)
			{
				if(bundleChoice.CurrentBundle != bundle)
				{
					bundleChoice.UnselectedBundle();
				}
			}
		}

		public void ConfirmBundle()
		{
			OnBundleChoiceConfirm?.Invoke(_currentBundleSelected, _currentBundleSelected.IsStartingGameBundle);
			_bundleChoiceGameObject.SetActive(false);
			UIBundleChoiceController.OnBundleSelected -= SelectCurrentBundle;
		}
	}
}

