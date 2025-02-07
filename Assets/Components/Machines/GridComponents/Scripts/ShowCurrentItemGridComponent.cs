using Components.Ingredients;
using Components.Machines.Behaviors;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Components.Machines
{
    public class ShowCurrentItemGridComponent : MachineGridComponent
    {
		[SerializeField] private RectTransform _rectTransform;
		[SerializeField] private Image _ingredientImage;
		[SerializeField] private Sprite _noItemSprite;

		[SerializeField] private SerializableDictionary<MachineType, Vector3> _canvasPositionByBehavior;

		private void Start()
		{
			_rectTransform.localPosition = _canvasPositionByBehavior[Machine.Template.Type];

		}
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Tab))
			{
				ShowCurrentItem(true);
			}
			else if (Input.GetKeyUp(KeyCode.Tab))
			{
				ShowCurrentItem(false);
			}
		}

		private void ShowCurrentItem(bool value)
		{
			_rectTransform.gameObject.SetActive(value);

			if(Machine.Behavior is MarchandMachineBehaviour marchandBehaviour)
			{
				_ingredientImage.sprite = marchandBehaviour.FavoriteIngredient.Icon;
			}
			else
			{
				_ingredientImage.sprite = Machine.OutIngredients.Count != 0 ? Machine.OutIngredients[0].Icon : _noItemSprite;
			}
		}
	}
}
