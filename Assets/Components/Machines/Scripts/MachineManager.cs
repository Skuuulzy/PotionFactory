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

        private readonly List<Machine> _machines = new();

        public Machine SelectedMachine { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            if (_machineTemplates.Count <= 0)
            {
                Debug.LogWarning("[MACHINES] No templates found.");
                return;
            }
            
            foreach (var machineTemplate in _machineTemplates)
            {
                var newMachine = new Machine(machineTemplate);
                _machines.Add(newMachine);
            }

            foreach (var machine in _machines)
            {
                MachineSelectorView selectorView = Instantiate(_machineSelectorView, _machineSelectorViewHolder);
                selectorView.Init(machine);
                selectorView.OnSelected += HandleMachineSelected;
            }
            
            // Init selected machine has the first 
            SelectedMachine = _machines[0];
        }

        private void HandleMachineSelected(Machine machine)
        {
            SelectedMachine = machine;
        }
    }
}