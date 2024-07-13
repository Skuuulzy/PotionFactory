using Components.Items;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Machines.Behaviors
{
    [CreateAssetMenu(fileName = "New Machine Behaviour", menuName = "Machines/Behavior/Extractor")]
    public class ExtractorMachineBehaviour : MachineBehavior
    {
        private ItemTemplate _itemTemplate;
        
		public void Init(ItemTemplate itemTemplate)
		{
            _itemTemplate = itemTemplate;
		}
        
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
                if (machine.OutPorts[0].Side.Opposite() != outMachine.InPorts[0].Side)
                {
                    return;
                }

                outMachine.TryGiveItemItem(new Item(_itemTemplate));
            }
        }
    }
}