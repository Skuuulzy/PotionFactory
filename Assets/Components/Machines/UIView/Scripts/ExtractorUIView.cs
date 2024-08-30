using TMPro;
using UnityEngine;

namespace Components.Machines.UIView
{
    public class ExtractorUIView : MachineUIViewBase
    {
        [SerializeField] private TMP_Dropdown _extractorResourceTypeDropdown;
        
        public override void Initialize(Machine machine)
        {
            base.Initialize(machine);
            
            
        }
    }
}