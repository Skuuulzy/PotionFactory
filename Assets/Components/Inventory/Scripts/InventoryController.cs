using Components.Bundle;
using Components.Machines;
using Components.Map;
using Components.Shop.ShopItems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VComponent.Tools.Singletons;

namespace Components.Inventory
{
    public class InventoryController : Singleton<InventoryController>
    {
		[SerializeField] private InventoryTemplate _defaultPlayerInventory;

		//Machine, consumables and relics
        private Dictionary<MachineTemplate, int> _playerMachinesDictionary;
		private List<ConsumableTemplate> _consumableTemplatesList;
		private List<RelicTemplate> _relicTemplatesList;
        public Dictionary<MachineTemplate, int> PlayerMachinesDictionary => _playerMachinesDictionary;
		public List<ConsumableTemplate> ConsumableTemplates => _consumableTemplatesList;
		public List<RelicTemplate> RelicTemplates => _relicTemplatesList;

		//Actions 
		public static Action<MachineTemplate,int> OnMachineAddedOrRemoved;
		public static Action<ConsumableTemplate> OnConsumableAdded;
		public static Action<ConsumableTemplate> OnConsumableRemoved;
		public static Action<RelicTemplate> OnRelicAdded;
		public static Action<RelicTemplate> OnRelicRemoved;


		//-------------------------------------------------------------------------------- MONO -------------------------------------------------------------------------------------
		private void Start()
		{
			_playerMachinesDictionary = new Dictionary<MachineTemplate, int>();
			_consumableTemplatesList = new List<ConsumableTemplate>();
			_relicTemplatesList = new List<RelicTemplate>();
			
			//Creating the player inventory with a basic template
			foreach(var kvp in _defaultPlayerInventory.MachineDictionary.ToDictionary())
			{
				AddMachineToPlayerInventory(kvp.Key, kvp.Value);
			}

			MapGenerator.OnMapChoiceConfirm += HandleMapChoiceConfirm;
		}

		private void OnDestroy()
		{
			MapGenerator.OnMapChoiceConfirm -= HandleMapChoiceConfirm;
		}



		//--------------------------------------------------------- ADDING AND REMOVING MACHINES CONSUMABLE AND RELICS --------------------------------------------------------------

		public void AddMachineToPlayerInventory(MachineTemplate machineTemplate, int numberOfMachine)
		{
			if (!_playerMachinesDictionary.TryAdd(machineTemplate, numberOfMachine))
			{
                _playerMachinesDictionary[machineTemplate] += numberOfMachine;
			}

			OnMachineAddedOrRemoved?.Invoke(machineTemplate, _playerMachinesDictionary[machineTemplate]);
		}

		public void DecreaseMachineToPlayerInventory(MachineTemplate machineTemplate, int numberOfMachine)
		{
			if (_playerMachinesDictionary.ContainsKey(machineTemplate))
			{
				_playerMachinesDictionary[machineTemplate] -= numberOfMachine;
				OnMachineAddedOrRemoved?.Invoke(machineTemplate, _playerMachinesDictionary[machineTemplate]);
			}
			else
			{
				Debug.LogError($"Can't remove this machine :{machineTemplate} because player doesn't has it in inventory");
			}
		}

		public int CountMachineOfType(MachineTemplate machineTemplate)
		{
			return _playerMachinesDictionary.GetValueOrDefault(machineTemplate, 0);
		}

		public void AddConsumableToPlayerInventory(ConsumableTemplate consumableTemplate, int v)
		{
			_consumableTemplatesList.Add(consumableTemplate);
			OnConsumableAdded?.Invoke(consumableTemplate);
		}

		public void RemoveConsumableFromPlayerInventory(ConsumableTemplate consumableTemplate)
		{
			if(_consumableTemplatesList.Contains(consumableTemplate) == false)
			{
				Debug.LogError("Should not remove this consumable : " + consumableTemplate);
				return;
			}
			_consumableTemplatesList.Remove(consumableTemplate );
			OnConsumableRemoved?.Invoke(consumableTemplate);
		}

		public void AddRelicToPlayerInventory(RelicTemplate relicTemplate, int v)
		{
			_relicTemplatesList.Add(relicTemplate);
			OnRelicAdded?.Invoke(relicTemplate);
		}

		public void RemoveRelicFromPlayerInventory(RelicTemplate relicTemplate)
		{
			if (_relicTemplatesList.Contains(relicTemplate) == false)
			{
				Debug.LogError("Should not remove this consumable : " + relicTemplate);
				return;
			}
			_relicTemplatesList.Remove(relicTemplate);
			OnRelicRemoved?.Invoke(relicTemplate);

		}

		private void HandleMapChoiceConfirm(IngredientsBundle bundle, bool isFirstChoice)
		{
			if(bundle.MachinesTemplateList.Count == 0)
			{
				return;
			}

			foreach(MachineTemplate machineTemplate in bundle.MachinesTemplateList)
			{
				AddMachineToPlayerInventory(machineTemplate, 1);
			}
		}
	}
}
