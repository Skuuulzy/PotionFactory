using System.Collections.Generic;
using Components.Ingredients;
using UnityEngine;

namespace Components.Machines.Behaviors
{
    [CreateAssetMenu(fileName = "New Machine Behaviour", menuName = "Component/Machines/Behavior/Merger")]
    public class MergerMachineBehavior : MachineBehavior
    {
        private Machine _lastMachineTakenFrom;
        

        public override bool CanTakeItem(Machine machine, Machine fromMachine, IngredientTemplate ingredient)
        {
            if (!machine.TryGetInMachine(out var connectedMachines))
            {
                return base.CanTakeItem(machine, fromMachine, ingredient);
            }

            // There is only one connected machine no need for specific behavior.
            if (connectedMachines.Count == 1 || _lastMachineTakenFrom == null)
            {
                if (!base.CanTakeItem(machine, fromMachine, ingredient)) 
                    return false;
                
                _lastMachineTakenFrom = fromMachine;
                return true;
            }

            if (_lastMachineTakenFrom == null)
            {
                _lastMachineTakenFrom = fromMachine;
                return true;
            }

            if (fromMachine == _lastMachineTakenFrom)
            {
                return false;
            }
            
            _lastMachineTakenFrom = fromMachine;
            return true;
        }
        
        public override void Process(Machine machine)
        {
            
        }
        
        public override void TryGiveOutIngredient(Machine machine)
        {
            if (machine.InIngredients.Count == 0)
            {
                return;
            }

            if (machine.TryGetOutMachines(out List<Machine> outMachines))
            {
                var outMachine = outMachines[0];

                if (outMachine.TryGiveIngredient(machine.InIngredients[0], machine))
                {
                    machine.RemoveItem(0);
                }
            }
        }
    }
}