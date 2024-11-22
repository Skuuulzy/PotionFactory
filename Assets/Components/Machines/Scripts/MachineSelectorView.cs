using Components.Inventory;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Components.Machines
{
    public class MachineSelectorView : MonoBehaviour
    {
        //[SerializeField] private TMP_Text _name;
        [SerializeField] private Image _background;
        [SerializeField] private TextMeshProUGUI _numberOfAvailableMAchineText;

        public static Action<MachineTemplate> OnSelected;
        
        private MachineTemplate _machine;

        public MachineTemplate Machine => _machine;

        public void Init(MachineTemplate machine, int value = 1)
        {
            _machine = machine;

            //_name.text = machine.Name;
            _background.sprite = machine.UIView;
            UpdateNumberOfAvailableMachine(value);
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
            _numberOfAvailableMAchineText.text = number.ToString();
		}
	}
}