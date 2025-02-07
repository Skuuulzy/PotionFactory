using UnityEngine;

namespace Components.Machines
{
    public class MarchandGridComponent : MachineGridComponent
    {
        private static readonly int PLAY = Animator.StringToHash("Play");
        
        [Header("Animation")] 
        [SerializeField] private Animator _sellFeedbackAnimator;

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
            _sellFeedbackAnimator.SetTrigger(PLAY);
        }
    }
}