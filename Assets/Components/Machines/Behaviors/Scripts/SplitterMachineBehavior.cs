using System.Collections.Generic;
using UnityEngine;

namespace Components.Machines.Behaviors
{
    [CreateAssetMenu(fileName = "New Machine Behaviour", menuName = "Component/Machines/Behavior/Splitter")]
    public class SplitterMachineBehavior : MachineBehavior
    {
        private Machine _lastMachineGivenTo;

        public override bool TryOutput()
        {
            // If there is only one out machine, the behaviour do not change.
            if (Machine.TryGetOutMachines(out List<Machine> outMachines))
            {
                if (outMachines.Count == 1)
                {
                    return base.TryOutput();
                }
            }

            if (_lastMachineGivenTo == null)
            {
                for (int i = 0; i < outMachines.Count; i++)
                {
                    var outMachine = outMachines[i];
                
                    if (outMachine.Behavior.TryInput(Machine.OutIngredients[0]))
                    {
                        _lastMachineGivenTo = outMachine;
                        Machine.RemoveItem(0);
                        break;
                    }
                }
                
                return true;
            }

            for (int i = 0; i < outMachines.Count; i++)
            {
                var outMachine = outMachines[i];

                if (outMachine == _lastMachineGivenTo)
                    continue;

                if (outMachine.Behavior.TryInput(Machine.OutIngredients[0]))
                {
                    _lastMachineGivenTo = outMachine;
                    Machine.RemoveItem(0);
                    break;
                }
            }

            _lastMachineGivenTo = null;
            return true;
        }
    }
}