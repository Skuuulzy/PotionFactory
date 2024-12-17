using Components.Inventory;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Components.Machines
{
    public class MachineSelectorView : MonoBehaviour
    {
        [SerializeField] private GrimoireInventoryButton _grimoireInventoryButton;

        public static Action<MachineTemplate> OnSelected;
        
        private MachineTemplate _machine;

        public MachineTemplate Machine => _machine;

        public void Init(MachineTemplate machine, int value = 1)
        {
            _machine = machine;

            _grimoireInventoryButton.InitMachine(machine, value);
        }

        
        //Call by machine in player inventory
        public void Select()
        {
            if(GrimoireController.Instance.PlayerMachinesDictionary[_machine] > 0)
			{
                OnSelected?.Invoke(_machine);
            }
        }

		public void UpdateNumberOfAvailableMachine(int number)
		{
            _grimoireInventoryButton.UpdateNumberOfAvailableMachine(number);
		}
	}
}