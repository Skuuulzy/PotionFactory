using System.Collections.Generic;
using UnityEngine;

namespace Components.Machines.Behaviors
{
    [CreateAssetMenu(fileName = "New Machine Behaviour", menuName = "Machines/Behavior/Conveyor")]
    public class ConveyorMachineBehavior : MachineBehavior
    {
        public override void Process(Machine machine)
        {
            if (machine.InIngredients.Count != 1)
            {
                return;
            }

            if (machine.TryGetOutMachines(out List<Machine> outMachines))
            {
                var outMachine = outMachines[0];
                
                if (outMachine.TryGiveItemItem(machine.InIngredients[0], machine))
                {
                    machine.RemoveItem(0);
                }
            }
        }
    }
}