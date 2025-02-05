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
                _ingredientController.CreateRepresentationFromTemplate(Machine.InIngredients);
                TranslateItem();
            }
            else if (Machine.InIngredients.Count == 0 && Machine.OutIngredients.Count == 0)
            {
                _ingredientController.DestroyRepresentation();
            }
        }

        private void TranslateItem()
        {
            StartCoroutine(MoveThroughTransforms(_ingredientController.IngredientView.transform, _translationPositions,TickSystem.Instance.CurrentTickDuration));
        }
        
        /// Coroutine to move the transform through multiple positions over a given total duration.
        private static IEnumerator MoveThroughTransforms(Transform objectToMove, List<Transform> targets, float duration)
        {
            objectToMove.position = targets[0].position;

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
        }
    }
}