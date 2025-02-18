using System;
using Components.Machines;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Components.Inventory
{
    public class UIGrimoireController : MonoBehaviour
    {
        [SerializeField] private MachineSelectorView _machineSelectorViewPrefab;
        [SerializeField] private ConsumableSelectorView _consumableSelectorViewPrefab;
        [SerializeField] private RelicSelectorView _relicSelectorViewPrefab;

        [SerializeField] private Transform _machineSelectorViewParent;
        [SerializeField] private Transform _consumableSelectorViewParent;
        [SerializeField] private Transform _relicSelectorViewParent;

        private List<MachineSelectorView> _inventoryMachinesList;
        private List<ConsumableSelectorView> _inventoryConsumableList;
        private List<RelicSelectorView> _inventoryRelicList;

        [SerializeField] private Animator _animator;

        public static Action<bool> OnEnableCleanMode;

        private void Start()
        {
            _inventoryMachinesList = new List<MachineSelectorView>();
            _inventoryConsumableList = new List<ConsumableSelectorView>();
            _inventoryRelicList = new List<RelicSelectorView>();

            GrimoireController.OnBaseInventoryGenerated += HandleBaseInventoryCreated;
            GrimoireController.OnMachineAddedOrRemoved += UpdateMachineUIView;
            GrimoireController.OnConsumableAdded += AddConsumableToInventory;
            GrimoireController.OnConsumableRemoved += RemoveConsumableFromInventory;
            GrimoireController.OnRelicAdded += AddRelicToInventory;
            GrimoireController.OnRelicRemoved += RemoveRelicFromInventory;
            StateController.OnStateStarted += HandleStateStarted;
        }

        private void OnDestroy()
        {
            GrimoireController.OnBaseInventoryGenerated -= HandleBaseInventoryCreated;
            GrimoireController.OnMachineAddedOrRemoved -= UpdateMachineUIView;
            GrimoireController.OnConsumableAdded -= AddConsumableToInventory;
            GrimoireController.OnConsumableRemoved -= RemoveConsumableFromInventory;
            GrimoireController.OnRelicAdded -= AddRelicToInventory;
            GrimoireController.OnRelicRemoved -= RemoveRelicFromInventory;
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

                case ShopItemType.CONSUMABLE:
                    _consumableSelectorViewParent.gameObject.SetActive(!_consumableSelectorViewParent.gameObject.activeSelf);
                    break;

                case ShopItemType.RELIC:
                    _relicSelectorViewParent.gameObject.SetActive(!_relicSelectorViewParent.gameObject.activeSelf);
                    break;
            }
        }

        private void HandleBaseInventoryCreated()
        {
            foreach (var machine in GrimoireController.Instance.PlayerMachinesDictionary)
            {
                UpdateMachineUIView(machine.Key, machine.Value);
            }
        }

        private void UpdateMachineUIView(MachineTemplate machineTemplate, int value)
        {
            //Search for an existing machine selector view to update the value
            foreach (MachineSelectorView machineSelectorView in _inventoryMachinesList)
            {
                if (machineSelectorView.Machine == machineTemplate)
                {
                    machineSelectorView.UpdateNumberOfAvailableMachine(value);
                    return;
                }
            }


            //Can't find a machine selector view so we instantiate a new one
            MachineSelectorView instantiateMachineSelectorView = Instantiate(_machineSelectorViewPrefab, _machineSelectorViewParent);
            instantiateMachineSelectorView.Init(machineTemplate, value);
            _inventoryMachinesList.Add(instantiateMachineSelectorView);

        }

        private void AddConsumableToInventory(ConsumableTemplate consumable)
        {
            ConsumableSelectorView instantiateConsumableSelectorView = Instantiate(_consumableSelectorViewPrefab, _consumableSelectorViewParent);
            instantiateConsumableSelectorView.Init(consumable);
            _inventoryConsumableList.Add(instantiateConsumableSelectorView);
        }

        private void RemoveConsumableFromInventory(ConsumableTemplate consumable)
        {
            for (int i = 0; i < _inventoryConsumableList.Count; i++)
            {
                if (_inventoryConsumableList[i].Consumable == consumable)
                {
                    Destroy(_inventoryConsumableList[i].gameObject);
                    _inventoryConsumableList.Remove(_inventoryConsumableList[i]);
                    return;
                }
            }

            Debug.LogError("Can't find the consumable template : " + consumable);
        }

        private void AddRelicToInventory(RelicTemplate relic)
        {
            RelicSelectorView instantiateRelicSelectorView = Instantiate(_relicSelectorViewPrefab, _relicSelectorViewParent);
            instantiateRelicSelectorView.Init(relic);
            _inventoryRelicList.Add(instantiateRelicSelectorView);
        }

        private void RemoveRelicFromInventory(RelicTemplate relic)
        {
            for (int i = 0; i < _inventoryRelicList.Count; i++)
            {
                if (_inventoryRelicList[i].Relic == relic)
                {
                    Destroy(_inventoryRelicList[i].gameObject);
                    _inventoryRelicList.Remove(_inventoryRelicList[i]);
                    return;
                }
            }

            Debug.LogError("Can't find the relic template : " + relic);
        }

        public void EnableCleanMode(bool enable)
        {
            OnEnableCleanMode?.Invoke(enable);
        }
    }
}
