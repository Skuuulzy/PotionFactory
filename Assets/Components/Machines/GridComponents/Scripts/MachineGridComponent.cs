using UnityEngine;

namespace Components.Machines
{
    public class MachineGridComponent : MonoBehaviour
    {
        protected Machine Machine;

        public void Initialize(Machine machine)
        {
            Machine = machine;
            
            SetUp();
        }

        protected virtual void SetUp() { }
    }
}