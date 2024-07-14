using UnityEngine;

namespace Components.Machines.Behaviors
{
    [CreateAssetMenu(fileName = "New Machine Behaviour", menuName = "Machines/Behavior/Conveyor")]
    public class ConveyorMachineBehavior : MachineBehavior
    {
        public override void Process(Machine machine)
        {
            if (machine.Items.Count != 1)
            {
                return;
            }

            if (machine.TryGetOutMachine(out Machine outMachine))
            {
                if (outMachine.TryGiveItemItem(machine.Items[0]))
                {
                    machine.RemoveItem(0);
                }
            }
        }
    }
}