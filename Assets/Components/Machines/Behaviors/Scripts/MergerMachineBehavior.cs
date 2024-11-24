using System.Collections.Generic;
using UnityEngine;

namespace Components.Machines.Behaviors
{
    [CreateAssetMenu(fileName = "New Machine Behaviour", menuName = "Machines/Behavior/Merger")]
    public class MergerMachineBehavior : MachineBehavior
    {
        private Machine _lastMachineTakenFrom;
        
        public override bool CanTakeItem(Machine machine, Machine fromMachine)
        {
            if (!machine.TryGetInMachine(out var connectedMachines))
            {
                return base.CanTakeItem(machine, fromMachine);
            }

            // There is only one connected machine no need for specific behavior.
            if (connectedMachines.Count == 1 || _lastMachineTakenFrom == null)
            {
                if (!base.CanTakeItem(machine, fromMachine)) 
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
            if (machine.Ingredients.Count == 0)
            {
                return;
            }

            if (machine.TryGetOutMachines(out List<Machine> outMachines))
            {
                var outMachine = outMachines[0];

                if (outMachine.TryGiveItemItem(machine.Ingredients[0], machine))
                {
                    machine.RemoveItem(0);
                }
            }
        }
    }
}