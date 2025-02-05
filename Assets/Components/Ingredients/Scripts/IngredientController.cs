using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Ingredients
{
	public class IngredientController : MonoBehaviour
	{
		[SerializeField] private SpriteRenderer _spriteRenderer;
		[SerializeField] private SpriteRenderer _favoriteSellerSpriteRenderer;
		[SerializeField] private Animator _animator;

		[SerializeField] private Transform _startTranslationPosition;
		[SerializeField] private Transform _endTranslationPosition;

		public void CreateRepresentationFromTemplate(List<IngredientTemplate> ingredientTemplate)
		{
			// TODO: See how we handle multiple items inside the machine ?
			_spriteRenderer.sprite = ingredientTemplate[0].Icon;
			_spriteRenderer.transform.position = _startTranslationPosition.position;
			//_animator.SetTrigger("AddItem");
		}

		public void CreateFavoriteSellerItemRepresentationFromTemplate(IngredientTemplate ingredientTemplate)
		{
			// TODO: See how we handle multiple items inside the machine ?
			_favoriteSellerSpriteRenderer.sprite = ingredientTemplate.Icon;
		}

		public void TranslateItem(float duration)
		{
			if (!_spriteRenderer.sprite)
			{
				return;
			}

			StartCoroutine(MoveTransform(_spriteRenderer.transform, 
				_startTranslationPosition.position, _endTranslationPosition.position, duration));
		}
		
		/// Coroutine to move the transform from one position to another over a given duration.
		private IEnumerator MoveTransform(Transform objectToMove, Vector3 startPosition, Vector3 targetPosition, float duration)
		{
			objectToMove.position = startPosition;

			float elapsedTime = 0f;

			while (elapsedTime < duration)
			{
				elapsedTime += Time.deltaTime;
				float t = elapsedTime / duration;
				objectToMove.position = Vector3.Lerp(startPosition, targetPosition, t);
				yield return null;
			}

			objectToMove.position = targetPosition; // Ensure it reaches exact target position at the end
		}

		public void DestroyRepresentation()
		{
			_spriteRenderer.sprite = null;
		}
	}
}