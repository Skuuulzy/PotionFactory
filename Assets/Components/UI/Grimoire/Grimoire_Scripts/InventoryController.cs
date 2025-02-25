using Components.Bundle;
using Components.Grid;
using Components.Machines;
using Components.Map;
using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using VComponent.Tools.Singletons;
using VTools.SoWorkflow.EventSystem;

namespace Components.Inventory
{
    public class InventoryController : Singleton<InventoryController>
    {
		[SerializeField] private InventoryTemplate _defaultPlayerInventory;

		//Machine, consumables and relics
        private Dictionary<MachineTemplate, int> _playerMachinesDictionary;

        public Dictionary<MachineTemplate, int> PlayerMachinesDictionary => _playerMachinesDictionary;

		//Actions 
		[SerializeField] private GameEvent OnShopUpdate;


		//-------------------------------------------------------------------------------- MONO -------------------------------------------------------------------------------------
		private void Start()
		{
			_playerMachinesDictionary = new Dictionary<MachineTemplate, int>();		


			BundleChoiceGenerator.OnBundleChoiceConfirm += HandleBundleChoice;
		}

		private void OnDestroy()
		{
			BundleChoiceGenerator.OnBundleChoiceConfirm -= HandleBundleChoice;
		}
		
		//--------------------------------------------------------- ADDING AND REMOVING MACHINES CONSUMABLE AND RELICS --------------------------------------------------------------

		public void AddGridObjectTemplateToInventory(GridObjectTemplate gridObject, int numberOfGridObject)
		{
			switch (gridObject)
			{
				case MachineTemplate machineTemplate:
					AddMachineToPlayerInventory(machineTemplate, numberOfGridObject);

                    break;
			}
			OnShopUpdate.Raise();

        }
        public void DecreaseGridObjectTemplate(GridObjectTemplate gridObject, int numberOfGridObject)
        {
            switch (gridObject)
            {
                case MachineTemplate machineTemplate:
                    DecreaseMachineToPlayerInventory(machineTemplate, numberOfGridObject);

                    break;
            }
            OnShopUpdate.Raise();
        }

		public int CountGridObject(GridObjectTemplate gridObject)
		{
			switch (gridObject)
			{
				case MachineTemplate machineTemplate:
					return _playerMachinesDictionary.Where(entry => entry.Key.Type == machineTemplate.Type).Sum(entry => entry.Value);
			}
			return 0;
		}

        private void AddMachineToPlayerInventory(MachineTemplate machineTemplate, int numberOfMachine)
		{
			if(machineTemplate.Type == MachineType.CONVEYOR)
			{
				machineTemplate = ScriptableObjectDatabase.GetScriptableObject<MachineTemplate>("ConveyorLeftToRight");
			}

			if (!_playerMachinesDictionary.TryAdd(machineTemplate, numberOfMachine))
			{
                _playerMachinesDictionary[machineTemplate] += numberOfMachine;
			}
		}

		private void DecreaseMachineToPlayerInventory(MachineTemplate machineTemplate, int numberOfMachine)
		{
			var matchingKey = _playerMachinesDictionary.Keys.FirstOrDefault(key => key.Type == machineTemplate.Type);

			if (matchingKey != null)
			{
				_playerMachinesDictionary[matchingKey] -= numberOfMachine;
			}
			else
			{
				Debug.LogError($"Can't remove this machine type: {machineTemplate.Type} because player doesn't have it in inventory");
			}
		}
	

		private void HandleBundleChoice(IngredientsBundle bundle, bool isFirstChoice)
		{
			if(bundle.MachinesTemplateList.Count == 0)
			{
				return;
			}
			
			//Creating the player inventory with a basic template
			foreach(var kvp in _defaultPlayerInventory.MachineDictionary.ToDictionary())
			{
                AddGridObjectTemplateToInventory(kvp.Key, kvp.Value);
			}

			foreach(MachineTemplate machineTemplate in bundle.MachinesTemplateList)
			{
				AddGridObjectTemplateToInventory(machineTemplate, 1);
			}
		}
	}
}
