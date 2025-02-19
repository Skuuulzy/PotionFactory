using Components.Bundle;
using Components.Machines;
using Components.Map;
using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VComponent.Tools.Singletons;

namespace Components.Inventory
{
    public class GrimoireController : Singleton<GrimoireController>
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
		public static Action OnBaseInventoryGenerated;


		//-------------------------------------------------------------------------------- MONO -------------------------------------------------------------------------------------
		private void Start()
		{
			_playerMachinesDictionary = new Dictionary<MachineTemplate, int>();
			_consumableTemplatesList = new List<ConsumableTemplate>();
			_relicTemplatesList = new List<RelicTemplate>();
			


			BundleChoiceGenerator.OnBundleChoiceConfirm += HandleBundleChoice;
		}

		private void OnDestroy()
		{
			BundleChoiceGenerator.OnBundleChoiceConfirm -= HandleBundleChoice;
		}
		
		//--------------------------------------------------------- ADDING AND REMOVING MACHINES CONSUMABLE AND RELICS --------------------------------------------------------------

		public void AddMachineToPlayerInventory(MachineTemplate machineTemplate, int numberOfMachine, bool inform = true)
		{
			if(machineTemplate.Type == MachineType.CONVEYOR)
			{
				machineTemplate = ScriptableObjectDatabase.GetScriptableObject<MachineTemplate>("ConveyorLeftToRight");
			}

			if (!_playerMachinesDictionary.TryAdd(machineTemplate, numberOfMachine))
			{
                _playerMachinesDictionary[machineTemplate] += numberOfMachine;
			}

			if (inform)
			{
				OnMachineAddedOrRemoved?.Invoke(machineTemplate, _playerMachinesDictionary[machineTemplate]);
			}
		}

		public void DecreaseMachineToPlayerInventory(MachineTemplate machineTemplate, int numberOfMachine)
		{
			var matchingKey = _playerMachinesDictionary.Keys.FirstOrDefault(key => key.Type == machineTemplate.Type);

			if (matchingKey != null)
			{
				_playerMachinesDictionary[matchingKey] -= numberOfMachine;
				OnMachineAddedOrRemoved?.Invoke(matchingKey, _playerMachinesDictionary[matchingKey]);
			}
			else
			{
				Debug.LogError($"Can't remove this machine type: {machineTemplate.Type} because player doesn't have it in inventory");
			}
		}

		public int CountMachineOfType(MachineType machineType)
		{
			return _playerMachinesDictionary.Where(entry => entry.Key.Type == machineType).Sum(entry => entry.Value);
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

		private void HandleBundleChoice(IngredientsBundle bundle, bool isFirstChoice)
		{
			if(bundle.MachinesTemplateList.Count == 0)
			{
				return;
			}
			
			//Creating the player inventory with a basic template
			foreach(var kvp in _defaultPlayerInventory.MachineDictionary.ToDictionary())
			{
				AddMachineToPlayerInventory(kvp.Key, kvp.Value, false);
			}
			
			OnBaseInventoryGenerated?.Invoke();

			foreach(MachineTemplate machineTemplate in bundle.MachinesTemplateList)
			{
				AddMachineToPlayerInventory(machineTemplate, 1);
			}
		}
	}
}
