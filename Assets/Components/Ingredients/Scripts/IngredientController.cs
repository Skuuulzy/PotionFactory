using System.Collections.Generic;
using UnityEngine;

namespace Components.Ingredients
{
	public class IngredientController : MonoBehaviour
	{
		[SerializeField] private SpriteRenderer _spriteRenderer;
		[SerializeField] private SpriteRenderer _favoriteSellerSpriteRenderer;

		public SpriteRenderer IngredientView => _spriteRenderer;

		public void CreateRepresentationFromTemplate(IngredientTemplate ingredientTemplate, Vector3 startPosition)
		{
			_spriteRenderer.transform.position = startPosition;
			_spriteRenderer.sprite = ingredientTemplate.Icon;
		}

		public void CreateFavoriteSellerItemRepresentationFromTemplate(IngredientTemplate ingredientTemplate)
		{
			// TODO: See how we handle multiple items inside the machine ?
			_favoriteSellerSpriteRenderer.sprite = ingredientTemplate.Icon;
		}

		public void DestroyRepresentation()
		{
			_spriteRenderer.sprite = null;
		}
	}
}