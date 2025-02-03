using System.Collections.Generic;
using UnityEngine;

namespace Components.Machines.Behaviors
{
    [CreateAssetMenu(fileName = "New Machine Behaviour", menuName = "Component/Machines/Behavior/Splitter")]
    public class SplitterMachineBehavior : MachineBehavior
    {
        private Machine _lastMachineGivenTo;

        protected override void Output()
        {
            // If there is only one out machine, the behaviour do not change.
            if (Machine.TryGetOutMachines(out List<Machine> outMachines))
            {
                if (outMachines.Count == 1)
                {
                    base.Output();
                    return;
                }
            }

            if (_lastMachineGivenTo == null)
            {
                for (int i = 0; i < outMachines.Count; i++)
                {
                    var outMachine = outMachines[i];
                
                    if (outMachine.TryInput(Machine.OutIngredients[0]))
                    {
                        _lastMachineGivenTo = outMachine;
                        Machine.RemoveItem(0);
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

                if (outMachine.TryInput(Machine.OutIngredients[0]))
                {
                    _lastMachineGivenTo = outMachine;
                    Machine.RemoveItem(0);
                    break;
                }
            }

            _lastMachineGivenTo = null;
        }
    }
}