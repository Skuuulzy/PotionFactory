using System;
using System.Collections.Generic;
using Components.Grid;
using Components.Tick;
using UnityEngine;

namespace Components.Machines
{
    [Serializable]
    public class Machine : ITickable
    {
        // ------------------------------------------------------------------------- PRIVATE FIELDS -------------------------------------------------------------------------
        
        [SerializeField] private SerializableDictionary<Side, Cell> _debugNeighbours;
        [SerializeField] private List<int> _items;
        
        private readonly MachineTemplate _template;
        private Dictionary<Side, Cell> _neighbours;
        private int _chainIndex;
        
        [SerializeField] private List<Side> _inPorts;
        [SerializeField] private List<Side> _outPorts;

        // ------------------------------------------------------------------------- PUBLIC FIELDS -------------------------------------------------------------------------
        
        public MachineTemplate Template => _template;
        public List<int> Items => _items;

        public virtual List<Side> InPorts => _inPorts;
        public virtual List<Side> OutPorts => _outPorts;
        
        public Action OnTick;
        public Action<bool> OnItemAdded;
        
        
        // ------------------------------------------------------------------------- CONSTRUCTORS -------------------------------------------------------------------------
        
        public Machine(MachineTemplate template, Dictionary<Side, Cell> neighbours, int rotation)
        {
            _template = template;
            _neighbours = neighbours;
            _debugNeighbours = new SerializableDictionary<Side, Cell>(neighbours);

            RotateMachine(rotation, true);
            
            _items = new List<int>();
        }
        
        // ------------------------------------------------------------------------- NEIGHBOURS -------------------------------------------------------------------------
        
        public bool TryGetOutMachine(out Machine connectedMachine)
        {
            if (_neighbours.ContainsKey(OutPorts[0]) && _neighbours[OutPorts[0]].MachineController != null)
            {
                connectedMachine = _neighbours[OutPorts[0]].MachineController.Machine;
                return true;
            }

            connectedMachine = null;
            return false;
        }
        
        public bool TryGetInMachine(out Machine connectedMachine)
        {
            if (_neighbours.ContainsKey(InPorts[0]) && _neighbours[InPorts[0]].MachineController != null)
            {
                connectedMachine = _neighbours[InPorts[0]].MachineController.Machine;
                return true;
            }

            connectedMachine = null;
            return false;
        }

        // ------------------------------------------------------------------------- ITEMS -------------------------------------------------------------------------
        
        public void AddItem()
        {
            OnItemAdded?.Invoke(true);
            Items.Add(66);
        }
        
        public bool TryGiveItemItem(int item)
        {
            // There is already too many items in the machine
            if (Template.MaxItemCount != -1 && Items.Count >= Template.MaxItemCount)
                return false;
            
            Items.Add(item);
            OnItemAdded?.Invoke(true);
            return true;
        }

        public void RemoveAllItems()
        {
            Items.Clear();
            OnItemAdded?.Invoke(false);
        }

        public void RemoveItem(int index)
        {
            Items.RemoveAt(index);
            OnItemAdded?.Invoke(false);
        }
        
        // ------------------------------------------------------------------------- PORTS -------------------------------------------------------------------------
        
        public void RotateMachine(int angle, bool clockwise)
        {
            if (!clockwise)
            {
                angle = 360 - angle;
            }

            angle %= 360;

            if (angle == 0)
            {
                _inPorts = _template.BaseInPorts;
                _outPorts = _template.BaseOutPorts;
                
                return;
            }
            
            var rotationMapping = GetRotationMapping(angle);
            _inPorts = RotatePorts(_template.BaseInPorts, rotationMapping);
            _outPorts = RotatePorts(_template.BaseOutPorts, rotationMapping);
        }

        private List<Side> RotatePorts(List<Side> ports, Dictionary<Side, Side> rotationMapping)
        {
            var rotatedPorts = new List<Side>();

            foreach (var port in ports)
            {
                rotatedPorts.Add(rotationMapping[port]);
            }

            return rotatedPorts;
        }

        private static Dictionary<Side, Side> GetRotationMapping(int angle)
        {
            var mapping = new Dictionary<Side, Side>();

            switch (angle)
            {
                case 90:
                    mapping[Side.NORTH] = Side.EAST;
                    mapping[Side.EAST] = Side.SOUTH;
                    mapping[Side.SOUTH] = Side.WEST;
                    mapping[Side.WEST] = Side.NORTH;
                    mapping[Side.NONE] = Side.NONE;
                    break;
                case 180:
                    mapping[Side.NORTH] = Side.SOUTH;
                    mapping[Side.EAST] = Side.WEST;
                    mapping[Side.SOUTH] = Side.NORTH;
                    mapping[Side.WEST] = Side.EAST;
                    mapping[Side.NONE] = Side.NONE;
                    break;
                case 270:
                    mapping[Side.NORTH] = Side.WEST;
                    mapping[Side.EAST] = Side.NORTH;
                    mapping[Side.SOUTH] = Side.EAST;
                    mapping[Side.WEST] = Side.SOUTH;
                    mapping[Side.NONE] = Side.NONE;
                    break;
                default:
                    throw new ArgumentException("Invalid rotation angle. Only 90, 180, and 270 degrees are allowed.");
            }

            return mapping;
        }
        
        public Side GetOppositeOutConnectionPort()
        {
            switch (OutPorts[0])
            {
                case Side.SOUTH:
                    return Side.NORTH;
                case Side.NORTH:
                    return Side.SOUTH;
                case Side.EAST:
                    return Side.WEST;
                case Side.WEST:
                    return Side.EAST;
                case Side.NONE:
                    return Side.NONE;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // ------------------------------------------------------------------------- TICK -------------------------------------------------------------------------
        
        public void Tick()
        {
            OnTick?.Invoke();
        }
    }
}