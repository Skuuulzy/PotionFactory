using Components.Ingredients;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Components.Machines.Behaviors
{
    [CreateAssetMenu(fileName = "New Machine Behaviour", menuName = "Machines/Behavior/Extractor")]
    public class ExtractorMachineBehaviour : MachineBehavior
    {
        [ShowInInspector, ReadOnly] private IngredientTemplate _ingredientTemplate;
        
		public void Init(IngredientTemplate ingredientTemplate)
		{
            _ingredientTemplate = ingredientTemplate;
		}
        
		public override void Process(Machine machine)
        {
            if (!_ingredientTemplate)
            {
                return;
            }
            
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