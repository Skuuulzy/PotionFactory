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
                if (machine.GetOppositeOutConnectionPort() != outMachine.InPorts[0])
                {
                    return;
                }
                
                if (outMachine.AcceptItem(machine.Items[0]))
                {
                    machine.RemoveItem(0);
                }
            }
        }
    }
}