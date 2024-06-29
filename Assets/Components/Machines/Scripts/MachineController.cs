using UnityEngine;

namespace Components.Machines
{
    public class MachineController : MonoBehaviour
    {
        [SerializeField] private Transform _3dViewHolder;
        
        private Machine _machine;

        public void Init(Machine machine)
        {
            _machine = machine;

            Instantiate(machine.Template.View, _3dViewHolder);
        }
    }
}