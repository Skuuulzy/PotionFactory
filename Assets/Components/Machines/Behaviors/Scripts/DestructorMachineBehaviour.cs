using UnityEngine;

namespace Components.Machines.Behaviors
{
    [CreateAssetMenu(fileName = "New Machine Behaviour", menuName = "Machines/Behavior/Destructor")]
    public class DestructorMachineBehaviour : MachineBehavior
    {
        public override void Process(Machine machine)
        {
            machine.RemoveAllItems();
        }
    }
}