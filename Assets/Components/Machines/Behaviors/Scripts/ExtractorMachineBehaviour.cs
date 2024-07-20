using Components.Items;
using UnityEngine;

namespace Components.Machines.Behaviors
{
    [CreateAssetMenu(fileName = "New Machine Behaviour", menuName = "Machines/Behavior/Extractor")]
    public class ExtractorMachineBehaviour : MachineBehavior
    {
        [SerializeField] private ItemTemplate _itemTemplate;
        
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
                outMachine.TryGiveItemItem(_itemTemplate);
            }
        }
    }
}