using System;
using System.Collections;
using System.Collections.Generic;
using Components.Ingredients;
using Components.Tick;
using UnityEngine;

namespace Components.Machines
{
    public class ConveyorGridComponent : MachineGridComponent
    {
        [Header("Ingredient")]
        [SerializeField] private IngredientController _ingredientController;
        
        [SerializeField] private List<Transform> _translationPositions;

        private bool _translating;
        
        protected override void SetUp()
        {
            Machine.OnSlotUpdated += TranslateItem;
        }

        private void OnDestroy()
        {
            Machine.OnSlotUpdated -= TranslateItem;
        }

        // ------------------------------------------------------------------------- ITEM -----------------------------------------------------------------------------
        
        private void ShowItem(bool show, IngredientTemplate ingredientToShow)
        {
            if (show)
            {
                _ingredientController.CreateRepresentationFromTemplate(ingredientToShow, _translationPositions[0].position);
            }
            else
            {
                _ingredientController.DestroyRepresentation();
            }
        }
        
        // ------------------------------------------------------------------------- TRANSLATE -----------------------------------------------------------------------------

        private void TranslateItem()
        {
            if (_translating)
            {
                return;
            }
            
            if (Machine.InIngredients.Count == 0)
            {
                return;
            }

            ShowItem(true, Machine.InIngredients[0]);
            
            // We reduce the translation time by a small offset to make sure that the target has reach is target at the next tick.
            var translationTime = TickSystem.Instance.CurrentTickDuration - 0.08f;
            StartCoroutine(MoveThroughTransforms(_ingredientController.IngredientView.transform, _translationPositions, translationTime));
        }
        
        /// Coroutine to move the transform through multiple positions over a given total duration.
        private IEnumerator MoveThroughTransforms(Transform objectToMove, List<Transform> targets, float duration)
        {
            _translating = true;
            
            // Making sure the object always have a Y world rotation of 0.
            Vector3 fixedRotation = objectToMove.eulerAngles;
            fixedRotation.y = 0f;
            objectToMove.rotation = Quaternion.Euler(fixedRotation);
            
            var segmentDuration = duration / (targets.Count - 1);
            
            for (int i = 0; i < targets.Count - 1; i++)
            {
                var startPosition = targets[i].position;
                var targetPosition = targets[i + 1].position;
                var elapsedTime = 0f;

                while (elapsedTime < segmentDuration)
                {
                    elapsedTime += Time.deltaTime;
                    var t = elapsedTime / segmentDuration;
                    objectToMove.position = Vector3.Lerp(startPosition, targetPosition, t);
                    yield return null;
                }

                objectToMove.position = targetPosition; // Ensure it reaches exact target position at the end of segment
            }
            
            if (Machine.InIngredients.Count == 0)
            {
                ShowItem(false, null);
            }
            
            _translating = false;
        }
    }
}