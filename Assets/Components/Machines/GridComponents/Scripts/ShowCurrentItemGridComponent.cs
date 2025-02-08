using Components.Machines.Behaviors;
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
			if (Input.GetKey(KeyCode.Tab))
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

			if (Machine.Behavior is MarchandMachineBehaviour marchandBehaviour)
			{
				if(marchandBehaviour.FavoriteIngredient == null)
				{
					return;
				}
				_ingredientImage.sprite = marchandBehaviour.FavoriteIngredient.Icon;
			}
			else if (Machine.Behavior is RecipeCreationBehavior recipeCreationBehavior)
			{
				if (!recipeCreationBehavior.ProcessingRecipe)
				{
					_ingredientImage.sprite = _noItemSprite;
					return;
				}

				_ingredientImage.sprite = recipeCreationBehavior.CurrentRecipe.OutIngredient.Icon;
			}
			else
			{
				_ingredientImage.sprite = Machine.OutIngredients.Count != 0 ? Machine.OutIngredients[0].Icon : _noItemSprite;
			}
		}
	}
}
