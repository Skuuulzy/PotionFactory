using System;
using System.Collections.Generic;
using Components.Grid;
using UnityEngine;

namespace Components.Machines
{
    [Serializable]
    public class Machine
    {
        #region DEBUG

        [SerializeField] private SerializableDictionary<Side, Cell> _debugNeighbours;

        #endregion DEBUG
        
        private readonly MachineTemplate _template;
        public MachineTemplate Template => _template;

        private Dictionary<Side, Cell> _neighbours;
        
        public List<Type> Items { get; }

        public Machine(MachineTemplate template, Dictionary<Side, Cell> neighbours)
        {
            _template = template;
            _neighbours = neighbours;
            _debugNeighbours = new SerializableDictionary<Side, Cell>(neighbours);

            Items = new List<Type>();
        }
        
        public bool AcceptItem(Type item)
        {
            // There is already too many items in the machine
            if (Items.Count >= Template.MaxItemCount)
                return false;
            
            Items.Add(item);
            return true;
        }
    }
}