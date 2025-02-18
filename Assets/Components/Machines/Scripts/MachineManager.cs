using System;
using VComponent.Tools.Singletons;

namespace Components.Machines
{
    public class MachineManager : Singleton<MachineManager>
    {        
        public MachineTemplate SelectedMachine { get; private set; }
        public static Action<MachineTemplate> OnChangeSelectedMachine;

        
        protected override void Awake()
        {
            base.Awake();
            
            MachineSelectorView.OnSelected += HandleMachineSelected;
        }

		private void OnDestroy()
		{
			MachineSelectorView.OnSelected -= HandleMachineSelected;
		}

		private void HandleMachineSelected(MachineTemplate machine)
        {
            SelectedMachine = machine;
            OnChangeSelectedMachine?.Invoke(machine);
        }
    }
}