using System;
using System.Collections.Generic;
using Components.Grid;
using Components.Machines.Behaviors;
using Components.Tick;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Components.Machines
{
    [Serializable]
    public class Machine : ITickable
    {
        // ------------------------------------------------------------------------- PRIVATE FIELDS -------------------------------------------------------------------------
        
        private readonly MachineTemplate _template;
        
        [ShowInInspector] private Dictionary<Side, Cell> _neighbours;
        [SerializeField] private List<int> _items;
        [SerializeField] private List<Side> _inPorts;
        [SerializeField] private List<Side> _outPorts;
        [SerializeField] private MachineController _controller;
        [SerializeField] private MachineBehavior _behavior;

        // ------------------------------------------------------------------------- PUBLIC FIELDS -------------------------------------------------------------------------
        
        public MachineTemplate Template => _template;
        public MachineController Controller => _controller;
        public MachineBehavior Behavior => _behavior;
        public List<int> Items => _items;

        public virtual List<Side> InPorts => _inPorts;
        public virtual List<Side> OutPorts => _outPorts;
        
        public Action OnTick;

        public Action<bool> OnItemAdded;
        
        // ------------------------------------------------------------------------- CONSTRUCTORS -------------------------------------------------------------------------
        
        public Machine(MachineTemplate template, Dictionary<Side, Cell> neighbours, int rotation, MachineController controller)
        {
            _template = template;
            _behavior = template.GetBehaviorClone();
            _neighbours = neighbours;
            _controller = controller;

            CalculatePortsViaRotation(rotation, false);
            
            _items = new List<int>();
        }
        
        // ------------------------------------------------------------------------- NEIGHBOURS -------------------------------------------------------------------------
        
        public bool TryGetOutMachine(out Machine connectedMachine)
        {
            if (_neighbours.ContainsKey(OutPorts[0]) && _neighbours[OutPorts[0]].MachineController != null)
            {
                connectedMachine = _neighbours[OutPorts[0]].MachineController.Machine;
                
                // The machines are not aligned.
                if (connectedMachine.InPorts[0] != GetOppositeConnectionSide(OutPorts[0]))
                {
                    return false;
                }
                
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
                
                // The machines are not aligned.
                if (connectedMachine.OutPorts[0] != GetOppositeConnectionSide(InPorts[0]))
                {
                    return false;
                }
                
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

        // ------------------------------------------------------------------------- TICK -------------------------------------------------------------------------
        
        public void Tick()
        {
            OnTick?.Invoke();
        }
        
        // ------------------------------------------------------------------------- PORTS & ROTATIONS -------------------------------------------------------------------------
        
        public void CalculatePortsViaRotation(int angle, bool clockwise)
        {
            if (!clockwise)
            {
                angle = 360 - angle;
            }

            angle %= 360;

            // The machine has no rotation so the ports are the base one.
            if (angle == 0)
            {
                _inPorts = _template.BaseInPorts;
                _outPorts = _template.BaseOutPorts;
                
                return;
            }
            
            var rotationMapping = GetRotationMapping(angle);
            
            _inPorts = GetPortFromRotation(_template.BaseInPorts, rotationMapping);
            _outPorts = GetPortFromRotation(_template.BaseOutPorts, rotationMapping);
        }

        private List<Side> GetPortFromRotation(List<Side> ports, Dictionary<Side, Side> rotationMapping)
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
        
        public Side GetOppositeConnectionSide(Side side)
        {
            switch (side)
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
    }
}