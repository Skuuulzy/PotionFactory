using System;
using System.Collections;
using Components.Ingredients;
using Components.Tick;
using UnityEngine;

namespace Components.Machines
{
    public class ConveyorGridComponent : MachineGridComponent
    {
        [Header("Ingredient")]
        [SerializeField] private IngredientController _ingredientController;
        
        [SerializeField] private Transform _startTranslationPosition;
        [SerializeField] private Transform _endTranslationPosition;
        
        protected override void SetUp()
        {
            Machine.OnItemAdded += ShowItem;
        }

        private void OnDestroy()
        {
            Machine.OnItemAdded -= ShowItem;
        }

        // ------------------------------------------------------------------------- ITEM -----------------------------------------------------------------------------
        private void ShowItem(bool show)
        {
            if (!Machine.Template.ShowItem)
            {
                return;
            }

            if (show)
            {
                _ingredientController.CreateRepresentationFromTemplate(Machine.InIngredients, _startTranslationPosition.position);
                TranslateItem();
            }
            else if (Machine.InIngredients.Count == 0 && Machine.OutIngredients.Count == 0)
            {
                _ingredientController.DestroyRepresentation();
            }
        }
        
        private void TranslateItem()
        {
            StartCoroutine(MoveTransform(_ingredientController.IngredientView.transform, _startTranslationPosition.position, _endTranslationPosition.position,
                TickSystem.Instance.CurrentTickDuration));
        }
        
        /// Coroutine to move the transform from one position to another over a given duration.
        private static IEnumerator MoveTransform(Transform objectToMove, Vector3 startPosition, Vector3 targetPosition, float duration)
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
    }
}