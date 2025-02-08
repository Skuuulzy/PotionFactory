using System.Collections;
using UnityEngine;

namespace Components.Machines
{
    public class MarchandGridComponent : MachineGridComponent
    {
        [SerializeField] private Transform _sellFeedbackHolder;
        [SerializeField] private GameObject _sellFeedback;

        protected override void SetUp()
        {
            Machine.OnItemSell += ShowSellFeedback;
        }

        private void OnDestroy()
        {
            Machine.OnItemSell -= ShowSellFeedback;
        }
        
        // ------------------------------------------------------------------------- SELL FEEDBACK -----------------------------------------------------------------------------
        private void ShowSellFeedback()
        {
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