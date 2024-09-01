using System.Collections.Generic;
using UnityEngine;

namespace Components.Ingredients
{
	public class IngredientController : MonoBehaviour
	{
		[SerializeField] private SpriteRenderer _spriteRenderer;
		
		public void CreateRepresentationFromTemplate(List<IngredientTemplate> ingredientTemplate)
		{
			// TODO: See how we handle multiple items inside the machine ?
			_spriteRenderer.sprite = ingredientTemplate[0].Icon;
		}

		public void DestroyRepresentation()
		{
			_spriteRenderer.sprite = null;
		}
	}
}