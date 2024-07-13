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
                // Detect if the port in is connected to the out .
                if (machine.GetOppositeConnectionSide(machine.OutPorts[0].Side) != outMachine.InPorts[0].Side)
                {
                    return;
                }
                
                if (outMachine.TryGiveItemItem(machine.Items[0]))
                {
                    machine.RemoveItem(0);
                }
            }
        }
    }
}