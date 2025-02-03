using System.Collections.Generic;
using Components.Ingredients;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Components.Machines.Behaviors
{
    [CreateAssetMenu(fileName = "New Machine Behaviour", menuName = "Component/Machines/Behavior/Extractor")]
    public class ExtractorMachineBehaviour : MachineBehavior
    {
        [ShowInInspector, ReadOnly] private IngredientTemplate _ingredientTemplate;
        [ShowInInspector, ReadOnly] private string _outMachineName;

        public IngredientTemplate IngredientTemplate => _ingredientTemplate;

        public void Init(IngredientTemplate ingredientTemplate)
		{
            _ingredientTemplate = ingredientTemplate;
		}
        
		public override void Process()
        {
            if (!_ingredientTemplate)
            {
                return;
            }
            
            CurrentTick++;
        }

        public override void TryGiveOutIngredient()
        {
            if (!CanProcess(CurrentTick))
            {
                return;
            }
            
            if (Machine.TryGetOutMachines(out List<Machine> outMachines))
            {
                var outMachine = outMachines[0];
                
                _outMachineName = outMachine.Controller.name;
                outMachine.TryGiveIngredient(_ingredientTemplate, Machine);
            }
            else
            {
                _outMachineName = "None";
            }
        }
    }
}