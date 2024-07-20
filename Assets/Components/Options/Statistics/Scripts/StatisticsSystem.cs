using System.Collections.Generic;
using UnityEngine;
using Components.Machines;
using Components.Grid;
using System;

public class StatisticsSystem : MonoBehaviour
{
    [SerializeField] private GridController _gridController;
    private Dictionary<string, int> _machineStatsDictionary;

    public Action<string, int, int> OnMachineAddedToStat;
    public Action<string, int, int> OnMachineRemovedToStat;

    private void Start()
    {
        _machineStatsDictionary = new Dictionary<string, int>();
        _gridController.OnMachineAdded += AddMachineToDictionary;
        _gridController.OnMachineRemoved += RemoveMachineToDictionary;
    }

	private void OnDestroy()
	{
        _gridController.OnMachineAdded -= AddMachineToDictionary;
        _gridController.OnMachineRemoved -= RemoveMachineToDictionary;
    }

    private void AddMachineToDictionary(Machine machine)
    {
        return;
        
        if (!_machineStatsDictionary.TryAdd(machine.Template.Name, 1))
        {
            _machineStatsDictionary[machine.Template.Name]++;
        }

        OnMachineAddedToStat?.Invoke(machine.Template.Name, _machineStatsDictionary[machine.Template.Name], machine.Behavior.ProcessTime);
    }

    private void RemoveMachineToDictionary(Machine machine)
	{
        if(_machineStatsDictionary[machine.Template.Name] == 0)
		{
            Debug.LogError("Try to remove this machine : " + machine.Template.Name + " but it should not");
		}
        _machineStatsDictionary[machine.Template.Name]--;
        
        OnMachineRemovedToStat?.Invoke(machine.Template.Name, _machineStatsDictionary[machine.Template.Name], machine.Behavior.ProcessTime);
    }
}