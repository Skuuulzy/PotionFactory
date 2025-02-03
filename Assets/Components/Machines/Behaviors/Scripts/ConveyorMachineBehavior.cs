using System.Collections.Generic;
using UnityEngine;

namespace Components.Machines.Behaviors
{
    [CreateAssetMenu(fileName = "New Machine Behaviour", menuName = "Component/Machines/Behavior/Conveyor")]
    public class ConveyorMachineBehavior : MachineBehavior
    {
        public override void Process()
        {
            
        }

        public override void TryGiveOutIngredient()
        {
            if (Machine.InIngredients.Count != 1)
            {
                return;
            }

            if (Machine.TryGetOutMachines(out List<Machine> outMachines))
            {
                var outMachine = outMachines[0];
                
                if (outMachine.TryGiveIngredient(Machine.InIngredients[0], Machine))
                {
                    Machine.RemoveItem(0);
                }
            }
        }
    }
}