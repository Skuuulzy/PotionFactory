using System;
using TMPro;
using UnityEngine;

namespace Components.Machines
{
    public class MachineSelectorView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _name;

        public Action<Machine> OnSelected;
        
        private Machine _machine;

        public void Init(Machine machine)
        {
            _machine = machine;

            _name.text = machine.Type.ToString();
        }

        public void Select()
        {
            Debug.Log($"[MACHINES] Selected: {_machine.Type}");
            
            OnSelected?.Invoke(_machine);
        }
    }
}