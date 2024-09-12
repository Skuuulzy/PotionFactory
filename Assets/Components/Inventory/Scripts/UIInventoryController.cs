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

		[SerializeField] private Transform _machineSelectorViewParent;
		private List<MachineSelectorView> _inventoryMachinesList;

		private void Awake()
		{
			_inventoryMachinesList = new List<MachineSelectorView>();
			InventoryController.OnMachineAddedOrRemoved += UpdateUIView;
		}

		private void OnDestroy()
		{
			InventoryController.OnMachineAddedOrRemoved -= UpdateUIView;

		}

		private void UpdateUIView(MachineTemplate machineTemplate, int value)
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
	}
}
