using System.Collections.Generic;
using UnityEngine;

namespace Components.Machines.Behaviors
{
    [CreateAssetMenu(fileName = "New Machine Behaviour", menuName = "Machines/Behavior/Conveyor")]
    public class ConveyorMachineBehavior : MachineBehavior
    {
        public override void Process(Machine machine)
        {
            if (machine.Ingredients.Count != 1)
            {
                return;
            }

            if (machine.TryGetOutMachines(out List<Machine> outMachines))
            {
                var outMachine = outMachines[0];
                
                if (outMachine.TryGiveItemItem(machine.Ingredients[0]))
                {
                    machine.RemoveItem(0);
                }
            }
        }
    }
}