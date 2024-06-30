using System.Collections.Generic;
using UnityEngine;
using VComponent.Tools.Singletons;

namespace Components.Machines
{
    public class MachineManager : Singleton<MachineManager>
    {
        [Header("Templates")]
        [SerializeField] private List<MachineTemplate> _machineTemplates;
        [Header("Selector View")]
        [SerializeField] private MachineSelectorView _machineSelectorView;
        [SerializeField] private Transform _machineSelectorViewHolder;
        
        public MachineTemplate SelectedMachine { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            if (_machineTemplates.Count <= 0)
            {
                Debug.LogWarning("[MACHINES] No templates found.");
                return;
            }

            foreach (var machine in _machineTemplates)
            {
                MachineSelectorView selectorView = Instantiate(_machineSelectorView, _machineSelectorViewHolder);
                selectorView.Init(machine);
                selectorView.OnSelected += HandleMachineSelected;
            }
            
            // Init selected machine has the first 
            SelectedMachine = _machineTemplates[0];
        }

        private void HandleMachineSelected(MachineTemplate machine)
        {
            SelectedMachine = machine;
        }
    }
}