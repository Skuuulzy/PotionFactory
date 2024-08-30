using TMPro;
using UnityEngine;

namespace Components.Machines.UIView
{
    public class MachineUIViewBase : MonoBehaviour
    {
        [SerializeField] private TMP_Text _machineName;
        
        protected Machine AssociatedMachine;

        public MachineType AssociatedType => AssociatedMachine.Template.Type;

        public virtual void Initialize(Machine machine)
        {
            AssociatedMachine = machine;
            _machineName.text = machine.Controller.name;
        }
    }
}