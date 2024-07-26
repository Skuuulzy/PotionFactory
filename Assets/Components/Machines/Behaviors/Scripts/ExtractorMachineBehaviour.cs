using Components.Items;
using UnityEngine;

namespace Components.Machines.Behaviors
{
    [CreateAssetMenu(fileName = "New Machine Behaviour", menuName = "Machines/Behavior/Extractor")]
    public class ExtractorMachineBehaviour : MachineBehavior
    {
        [SerializeField] private IngredientTemplate _ingredientTemplate;
        
		public void Init(IngredientTemplate ingredientTemplate)
		{
            _ingredientTemplate = ingredientTemplate;
		}
        
		public override void Process(Machine machine)
        {
            CurrentTick++;

            if (!CanProcess(CurrentTick))
            {
                return;
            }
            
            if (machine.TryGetOutMachine(out Machine outMachine))
            {
                outMachine.TryGiveItemItem(_ingredientTemplate);
            }
        }
    }
}