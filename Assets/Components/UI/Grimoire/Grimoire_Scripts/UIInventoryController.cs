using System;
using Components.Machines;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Components.Grid;
using static UnityEngine.Rendering.DebugUI;
using System.Linq;


namespace Components.Inventory
{
    public class UIInventoryController : MonoBehaviour
    {
        [SerializeField] private MachineSelectorView _machineSelectorViewPrefab;

        [SerializeField] private Transform _machineSelectorViewParent;

        private List<MachineSelectorView> _inventoryMachinesList;

        [SerializeField] private Animator _animator;

        public static Action<bool> OnEnableCleanMode;

        private void Start()
        {
            _inventoryMachinesList = new List<MachineSelectorView>();
            StateController.OnStateStarted += HandleStateStarted;
        }

        private void OnDestroy()
        {
            StateController.OnStateStarted -= HandleStateStarted;
        }

        private void HandleStateStarted(BaseState state)
        {
            if (state is ResolutionFactoryState)
            {
                _animator.SetTrigger("Apparition");
            }
        }

        //Call by Grimoire button
        public void ToggleInventory(int type)
        {
            switch ((ShopItemType)type)
            {
                case ShopItemType.MACHINE:
                    _machineSelectorViewParent.gameObject.SetActive(!_machineSelectorViewParent.gameObject.activeSelf);
                    break;
            }
        }

        public void UpdateGrimoireView()
        {
            UpdateMachineUIView();
        }

        private void UpdateMachineUIView()
        {
            var machineDictionary = InventoryController.Instance.PlayerMachinesDictionary;

            foreach (var machine in machineDictionary)
            {
                var machineSelectorView = _inventoryMachinesList.Find(x => x.Machine == machine.Key);
                if (machineSelectorView != null)
                {
                    machineSelectorView.UpdateMachineNumber(machine.Value);
                }
                else
                {
                    MachineSelectorView instantiateMachineSelectorView = Instantiate(_machineSelectorViewPrefab, _machineSelectorViewParent);
                    instantiateMachineSelectorView.Init(machine.Key, machine.Value);
                    _inventoryMachinesList.Add(instantiateMachineSelectorView);
                }
            }
        }

        public void EnableCleanMode(bool enable)
        {
            OnEnableCleanMode?.Invoke(enable);
        }
    }
}
