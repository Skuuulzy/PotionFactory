using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Components.Machines
{
    public class MachineSelectorView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _name;
        [SerializeField] private Image _background;

        public Action<MachineTemplate> OnSelected;
        
        private MachineTemplate _machine;

        public void Init(MachineTemplate machine)
        {
            _machine = machine;

            _name.text = machine.Name.ToString();
            _background.sprite = machine.UIView;
        }

        public void Select()
        {
            Debug.Log($"[MACHINES] Selected: {_machine.Name}");
            
            OnSelected?.Invoke(_machine);
        }
    }
}