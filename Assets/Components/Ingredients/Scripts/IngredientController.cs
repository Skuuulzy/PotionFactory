using System.Collections.Generic;
using UnityEngine;

namespace Components.Ingredients
{
	public class IngredientController : MonoBehaviour
	{
		[SerializeField] private SpriteRenderer _spriteRenderer;
		[SerializeField] private SpriteRenderer _favoriteSellerSpriteRenderer;
		[SerializeField] private Animator _animator;
		
		public void CreateRepresentationFromTemplate(List<IngredientTemplate> ingredientTemplate)
		{
			// TODO: See how we handle multiple items inside the machine ?
			_spriteRenderer.sprite = ingredientTemplate[0].Icon;
			_animator.SetTrigger("AddItem");
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