using System.Collections.Generic;
using UnityEngine;

namespace Components.Machines.Behaviors
{
    [CreateAssetMenu(fileName = "New Machine Behaviour", menuName = "Component/Machines/Behavior/Splitter")]
    public class SplitterMachineBehavior : MachineBehavior
    {
        private Machine _lastMachineGivenTo;

        public override void Process()
        {
            
        }

        public override void TryGiveOutIngredient()
        {
            if (Machine.InIngredients.Count == 0)
            {
                return;
            }

            if (!Machine.TryGetOutMachines(out List<Machine> outMachines)) 
                return;

            if (outMachines.Count == 1)
            {
                if (outMachines[0].TryGiveIngredient(Machine.InIngredients[0], Machine))
                {
                    Machine.RemoveItem(0);
                }
                
                return;
            }

            if (_lastMachineGivenTo == null)
            {
                for (int i = 0; i < outMachines.Count; i++)
                {
                    var outMachine = outMachines[i];
                
                    if (outMachine.TryGiveIngredient(Machine.InIngredients[0], Machine))
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

                if (outMachine.TryGiveIngredient(Machine.InIngredients[0], Machine))
                {
                    _lastMachineGivenTo = outMachine;
                    Machine.RemoveItem(0);
                    return;
                }
            }

            _lastMachineGivenTo = null;
        }
    }
}