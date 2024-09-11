using Components.Machines;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VComponent.Tools.Singletons;

namespace Components.Inventory
{
    public class InventoryController : Singleton<InventoryController>
    {
		[SerializeField] private InventoryTemplate _defaultPlayerInventory;
        private Dictionary<MachineTemplate, int> _playerMachinesDictionary;
        public Dictionary<MachineTemplate, int> PlayerMachinesDictionary => _playerMachinesDictionary;

		public static Action<MachineTemplate,int> OnMachineAddedOrRemoved;

		private void Start()
		{
			_playerMachinesDictionary = new Dictionary<MachineTemplate, int>();
			//Creating the player inventory with a basic template
			foreach(var kvp in _defaultPlayerInventory.MachineDictionary.ToDictionary())
			{
				AddMachineToPlayerInventory(kvp.Key, kvp.Value);
			}
		}

		public void AddMachineToPlayerInventory(MachineTemplate machineTemplate, int numberOfMachine)
		{
			if (_playerMachinesDictionary.ContainsKey(machineTemplate))
			{
                _playerMachinesDictionary[machineTemplate] += numberOfMachine;
			}
			else
			{
				_playerMachinesDictionary.Add(machineTemplate, numberOfMachine);
			}

			OnMachineAddedOrRemoved?.Invoke(machineTemplate, _playerMachinesDictionary[machineTemplate]);
		}

		public void RemoveMachineToPlayerInventory(MachineTemplate machineTemplate, int numberOfMachine)
		{
			if (_playerMachinesDictionary.ContainsKey(machineTemplate))
			{
				_playerMachinesDictionary[machineTemplate] -= numberOfMachine;
			}
			else
			{
				Debug.LogError($"Can't remove this machine :{machineTemplate} because player doesn't has it in inventory");
			}

			OnMachineAddedOrRemoved?.Invoke(machineTemplate, _playerMachinesDictionary[machineTemplate]);
		}
	}
}
