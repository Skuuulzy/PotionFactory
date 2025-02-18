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
                _ingredientController.DestroyRepresentation();
                return;
            }

            ShowItem(true, Machine.InIngredients[0]);

            //StartCoroutine(MoveThroughTransforms(_ingredientController.IngredientView.transform, _translationPositions,TickSystem.Instance.CurrentTickDuration, Machine.Template.ProcessTime));
        }

        /// Coroutine to move the transform through multiple positions over a given total duration (based on tick system).
        private IEnumerator MoveThroughTransforms(Transform objectToMove, List<Transform> targets, float tickDuration, int tickCount)
        {
            _translating = true;

            // Making sure the object always has a Y world rotation of 0.
            Vector3 fixedRotation = objectToMove.eulerAngles;
            fixedRotation.y = 0f;
            objectToMove.rotation = Quaternion.Euler(fixedRotation);

            var totalDuration = tickDuration * tickCount; // Example: 0.1s * 10 = 1s
            var segmentDuration = totalDuration / (targets.Count - 1);
            var ticksPerSegment = Mathf.CeilToInt(segmentDuration / tickDuration);

            for (int i = 0; i < targets.Count - 1; i++)
            {
                var startPosition = targets[i].position;
                var targetPosition = targets[i + 1].position;

                for (int tick = 0; tick < ticksPerSegment; tick++)
                {
                    if (Machine.InIngredients.Count == 0)
                    {
                        ShowItem(false, null);
                    }

                    float t = (float)(tick + 1) / ticksPerSegment;
                    objectToMove.position = Vector3.Lerp(startPosition, targetPosition, t);
                    yield return new WaitForSeconds(tickDuration); // Wait for the tick duration
                }

                objectToMove.position = targetPosition; // Ensure it reaches the exact target position at the end of segment
            }



            _translating = false;
        }
    }
}