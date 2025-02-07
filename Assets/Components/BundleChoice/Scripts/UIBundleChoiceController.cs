using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Components.Bundle
{
	public class UIBundleChoiceController : MonoBehaviour
	{
		[SerializeField] private Transform _bundleIngredientsAndMachinesTransform;
		[SerializeField] private UIBundleView _bundleViewPrefab;
		[SerializeField] private GameObject _bundleSelectedFeedbackGO;
		[SerializeField] private Button _selectButton;


		public IngredientsBundle CurrentBundle;

		public static Action<IngredientsBundle> OnBundleSelected;
		public void SetInfos(IngredientsBundle bundle)
		{
			CurrentBundle = bundle;

			foreach (Transform child in _bundleIngredientsAndMachinesTransform)
			{
				Destroy(child.gameObject);
			}

			foreach (var machineTemplate in CurrentBundle.MachinesTemplateList)
			{
				UIBundleView bundleView = Instantiate(_bundleViewPrefab, _bundleIngredientsAndMachinesTransform);
				bundleView.SetInfos(machineTemplate.UIView, machineTemplate.Name);
			}

			foreach (var ingredientsTemplate in CurrentBundle.IngredientsTemplatesList)
			{
				UIBundleView bundleView = Instantiate(_bundleViewPrefab, _bundleIngredientsAndMachinesTransform);
				bundleView.SetInfos(ingredientsTemplate.Icon, ingredientsTemplate.Name);
			}

			UnselectedBundle();
		}

		public void SelectBundle()
		{
			OnBundleSelected?.Invoke(CurrentBundle);
			_bundleSelectedFeedbackGO.SetActive(true);
		}

		public void UnselectedBundle()
		{
			_bundleSelectedFeedbackGO.SetActive(false);
		}

	}
}

