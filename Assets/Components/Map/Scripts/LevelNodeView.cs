using Components.Bundle;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Components.Map
{
	public class LevelNodeView : MonoBehaviour
    {
		[Header("View infos")]
		[SerializeField] private Image _ingredientImage;
		[SerializeField] private GameObject _starterBundleGameObject;
		[SerializeField] private List<Image> _starterBundleIngredientsImageList;
		[SerializeField] private List<Image> _starterBundleMachineImageList;

		private bool _isStarterBundle;

		public void Init(IngredientsBundle ingredientsBundle)
		{
			if (!ingredientsBundle.IsStartingGameBundle)
			{
				_isStarterBundle = false;

				_ingredientImage.gameObject.SetActive(true);
				_starterBundleGameObject.SetActive(false);
				_ingredientImage.sprite = ingredientsBundle.IngredientsTemplatesList[0].Icon;
			}
			else
			{
				_isStarterBundle = true;
				_ingredientImage.gameObject.SetActive(false);

				for (int i = 0; i < ingredientsBundle.IngredientsTemplatesList.Count; i++)
				{
					_starterBundleIngredientsImageList[i].gameObject.SetActive(true);
					_starterBundleIngredientsImageList[i].sprite = ingredientsBundle.IngredientsTemplatesList[i].Icon;
				}

				for (int i = 0; i < ingredientsBundle.MachinesTemplateList.Count; i++)
				{
					_starterBundleMachineImageList[i].gameObject.SetActive(true);
					_starterBundleMachineImageList[i].sprite = ingredientsBundle.MachinesTemplateList[i].UIView;
				}
			}
		}

		public void DisplayStarterBundle(bool value)
		{
			if (_isStarterBundle)
			{
				_starterBundleGameObject.SetActive(value);
			}
		}

		public void HandleResetLevelNode()
		{
			_ingredientImage.gameObject.SetActive(false);
		}
	}

}

