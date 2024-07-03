using UnityEngine;

namespace Components.Machines.Behaviors
{
    [CreateAssetMenu(fileName = "New Machine Behaviour", menuName = "Machines/Behavior/Extractor")]
    public class ExtractorMachineBehaviour : MachineBehavior
    {
        public override void Process(Machine machine)
        {
            CurrentTick++;

            if (!CanProcess(CurrentTick))
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

                outMachine.TryGiveItemItem(66);
            }
        }
    }
}