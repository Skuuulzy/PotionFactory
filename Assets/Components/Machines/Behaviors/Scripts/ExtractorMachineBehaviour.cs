using Components.Items;
using UnityEngine;

namespace Components.Machines.Behaviors
{
    [CreateAssetMenu(fileName = "New Machine Behaviour", menuName = "Machines/Behavior/Extractor")]
    public class ExtractorMachineBehaviour : MachineBehavior
    {
        [SerializeField] private ItemTemplate _itemTemplate;
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
                if (machine.GetOppositeConnectionSide(machine.OutPorts[0]) != outMachine.InPorts[0])
                {
                    return;
                }

                outMachine.TryGiveItemItem(new Item(_itemTemplate));
            }
        }
    }
}