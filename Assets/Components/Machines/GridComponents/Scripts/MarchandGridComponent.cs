using System.Collections;
using Components.Ingredients;
using Components.Machines.Behaviors;
using UnityEngine;

namespace Components.Machines
{
    public class MarchandGridComponent : MachineGridComponent
    {
        [SerializeField] private Transform _sellFeedbackHolder;
        [SerializeField] private GameObject _sellFeedback;

        private MarchandMachineBehaviour _marchandBehaviour;
        
        protected override void SetUp()
        {
            if (Machine.Behavior is MarchandMachineBehaviour marchandBehaviour)
            {
                _marchandBehaviour = marchandBehaviour;
                MarchandMachineBehaviour.OnIngredientSold += ShowSellFeedback;
            }
        }

        private void OnDestroy()
        {
            MarchandMachineBehaviour.OnIngredientSold -= ShowSellFeedback;
        }
        
        // ------------------------------------------------------------------------- SELL FEEDBACK -----------------------------------------------------------------------------
        private void ShowSellFeedback(MarchandMachineBehaviour sender, IngredientTemplate ingredientTemplate)
        {
            if (_marchandBehaviour != sender)
            {
                return;
            }
            
            // TODO: Change for pool system
            var feedback = Instantiate(_sellFeedback, _sellFeedbackHolder);
            StartCoroutine(DestroyAfter(feedback, 1f));
        }

        private IEnumerator DestroyAfter(GameObject objectToDestroy , float time)
        {
            yield return new WaitForSeconds(time);
            Destroy(objectToDestroy);
        }
    }
}