using Components.Machines;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Components.Inventory
{
    public class UIInventoryController : MonoBehaviour
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

		private void Awake()
		{
			_inventoryMachinesList = new List<MachineSelectorView>();
			_inventoryConsumableList = new List<ConsumableSelectorView>();
			_inventoryRelicList = new List<RelicSelectorView>();

			InventoryController.OnMachineAddedOrRemoved += UpdateMachineUIView;
			InventoryController.OnConsumableAdded += AddConsumableToInventory;
			InventoryController.OnConsumableRemoved += RemoveConsumableFromInventory;
			InventoryController.OnRelicAdded += AddRelicToInventory;
			InventoryController.OnRelicRemoved += RemoveRelicFromInventory;
		}

		private void OnDestroy()
		{
			InventoryController.OnMachineAddedOrRemoved -= UpdateMachineUIView;
			InventoryController.OnConsumableAdded -= AddConsumableToInventory;
			InventoryController.OnConsumableRemoved -= RemoveConsumableFromInventory;
			InventoryController.OnRelicAdded -= AddRelicToInventory;
			InventoryController.OnRelicRemoved -= RemoveRelicFromInventory;



		}

		private void UpdateMachineUIView(MachineTemplate machineTemplate, int value)
		{

			//Search for an existing machine selector view to update the value
			foreach(MachineSelectorView machineSelectorView in _inventoryMachinesList)
			{
				if(machineSelectorView.Machine == machineTemplate)
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
			for(int i = 0; i < _inventoryConsumableList.Count; i++)
			{
				if(_inventoryConsumableList[i].Consumable == consumable)
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
	}
}
