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
        [SerializeField] private List<int> _items;

        #endregion DEBUG
        
        private readonly MachineTemplate _template;
        private Dictionary<Side, Cell> _neighbours;
        private int _chainIndex;
        
        public MachineTemplate Template => _template;
        public List<int> Items => _items;

        public Machine(MachineTemplate template, Dictionary<Side, Cell> neighbours)
        {
            _template = template;
            _neighbours = neighbours;
            _debugNeighbours = new SerializableDictionary<Side, Cell>(neighbours);


            
            _items = new List<int>();
        }

        #region NEIGHBOURG METHODS

        public bool TryGetOutMachine(out Machine connectedMachine)
        {
            if (_neighbours[_template.OutPorts[0]].MachineController != null)
            {
                connectedMachine = _neighbours[_template.OutPorts[0]].MachineController.Machine;
                return true;
            }

            connectedMachine = null;
            return false;
        }
        
        public bool TryGetInMachine(out Machine connectedMachine)
        {
            if (_neighbours[_template.InPorts[0]] != null)
            {
                connectedMachine = _neighbours[_template.OutPorts[0]].MachineController.Machine;
                return true;
            }

            connectedMachine = null;
            return false;
        }

        #endregion

        public void AddItem()
        {
            Items.Add(66);
        }
        
        public bool AcceptItem(int item)
        {
            // There is already too many items in the machine
            if (Items.Count >= Template.MaxItemCount)
                return false;
            
            Items.Add(item);
            return true;
        }
    }
}