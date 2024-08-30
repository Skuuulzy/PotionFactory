using TMPro;
using UnityEngine;

namespace Components.Machines.UIView
{
    public class MachineUIViewBase : MonoBehaviour
    {
        [SerializeField] private TMP_Text _machineName;

        private MachineType _associatedType;

        public MachineType AssociatedType => _associatedType;

        public virtual void Initialize(Machine machine)
        {
            _associatedType = machine.Template.Type;
            _machineName.text = machine.Controller.name;
        }
    }
}