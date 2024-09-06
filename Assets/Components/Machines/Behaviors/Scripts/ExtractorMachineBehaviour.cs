using System.Collections.Generic;
using Components.Ingredients;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Components.Machines.Behaviors
{
    [CreateAssetMenu(fileName = "New Machine Behaviour", menuName = "Machines/Behavior/Extractor")]
    public class ExtractorMachineBehaviour : MachineBehavior
    {
        [ShowInInspector, ReadOnly] private IngredientTemplate _ingredientTemplate;
        [ShowInInspector, ReadOnly] private string _outMachineName;
        
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
            
            if (machine.TryGetOutMachines(out List<Machine> outMachines))
            {
                var outMachine = outMachines[0];
                
                _outMachineName = outMachine.Controller.name;
                outMachine.TryGiveItemItem(_ingredientTemplate);
            }
            else
            {
                _outMachineName = "None";
            }
        }
    }
}