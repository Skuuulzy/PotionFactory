using System.Collections.Generic;
using UnityEngine;

namespace Components.Machines.Behaviors
{
    [CreateAssetMenu(fileName = "New Machine Behaviour", menuName = "Machines/Behavior/Splitter")]
    public class SplitterMachineBehavior : MachineBehavior
    {
        private Machine _lastMachineGivenTo;

        public override void Process(Machine machine)
        {
            
        }

        public override void TryGiveOutIngredient(Machine machine)
        {
            if (machine.InIngredients.Count == 0)
            {
                return;
            }

            if (!machine.TryGetOutMachines(out List<Machine> outMachines)) 
                return;

            if (outMachines.Count == 1)
            {
                if (outMachines[0].TryGiveIngredient(machine.InIngredients[0], machine))
                {
                    machine.RemoveItem(0);
                }
                
                return;
            }

            if (_lastMachineGivenTo == null)
            {
                for (int i = 0; i < outMachines.Count; i++)
                {
                    var outMachine = outMachines[i];
                
                    if (outMachine.TryGiveIngredient(machine.InIngredients[0], machine))
                    {
                        _lastMachineGivenTo = outMachine;
                        machine.RemoveItem(0);
                        break;
                    }
                }
                
                return;
            }

            for (int i = 0; i < outMachines.Count; i++)
            {
                var outMachine = outMachines[i];

                if (outMachine == _lastMachineGivenTo)
                    continue;

                if (outMachine.TryGiveIngredient(machine.InIngredients[0], machine))
                {
                    _lastMachineGivenTo = outMachine;
                    machine.RemoveItem(0);
                    return;
                }
            }

            _lastMachineGivenTo = null;
        }
    }
}