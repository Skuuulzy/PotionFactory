using Components.Machines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Inventory
{
    [CreateAssetMenu(fileName = "New Invetory Template", menuName = "Component/Inventory/Inventory Template")]
    public class InventoryTemplate : ScriptableObject
    {
        [SerializeField] private SerializableDictionary<MachineTemplate, int> _machineDictionary;

        public SerializableDictionary<MachineTemplate, int> MachineDictionary => _machineDictionary;
    }
}