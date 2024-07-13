using System;
using System.Collections.Generic;
using Components.Grid;
using Components.Items;
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
        [SerializeField] private List<Item> _items;
        [SerializeField] private List<Port> _inPorts;
        [SerializeField] private List<Port> _outPorts;
        [SerializeField] private MachineController _controller;
        [SerializeField] private MachineBehavior _behavior;

        // ------------------------------------------------------------------------- PUBLIC FIELDS -------------------------------------------------------------------------
        public MachineTemplate Template => _template;
        public MachineController Controller => _controller;
        public MachineBehavior Behavior => _behavior;
        public List<Item> Items => _items;

        public virtual List<Port> InPorts => _inPorts;
        public virtual List<Port> OutPorts => _outPorts;
        
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
            
            _items = new List<Item>();
        }
        
        // ------------------------------------------------------------------------- NEIGHBOURS -------------------------------------------------------------------------
        public bool TryGetOutMachine(out Machine connectedMachine)
        {
            if (_neighbours.ContainsKey(OutPorts[0].Side) && _neighbours[OutPorts[0].Side].MachineController != null)
            {
                connectedMachine = _neighbours[OutPorts[0].Side].MachineController.Machine;
                
                // The machines are not aligned.
                if (connectedMachine.InPorts[0].Side != GetOppositeConnectionSide(OutPorts[0].Side))
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
            if (_neighbours.ContainsKey(InPorts[0].Side) && _neighbours[InPorts[0].Side].MachineController != null)
            {
                connectedMachine = _neighbours[InPorts[0].Side].MachineController.Machine;
                
                // The machines are not aligned.
                if (connectedMachine.OutPorts[0].Side != GetOppositeConnectionSide(InPorts[0].Side))
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
            //Items.Add(66);
        }
        
        public bool TryGiveItemItem(Item item)
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

            _inPorts = RotatePorts(_template.BaseInPorts, rotationMapping, angle, Template.Dimension);
            _outPorts = RotatePorts(_template.BaseOutPorts, rotationMapping, angle, Template.Dimension);
        }
        
        private List<Port> RotatePorts(List<Port> ports, Dictionary<Side, Side> rotationMapping, int angle, Vector2Int dimensions)
        {
            var rotatedPorts = new List<Port>();

            foreach (var port in ports)
            {
                var newPosition = RotatePosition(port.Position, angle, dimensions);
                var newSide = rotationMapping[port.Side];
                rotatedPorts.Add(new Port(newSide, newPosition));
            }

            return rotatedPorts;
        }
        
        private Vector2Int RotatePosition(Vector2Int position, int angle, Vector2Int dimensions)
        {
            switch (angle)
            {
                case 90:
                    return new Vector2Int(dimensions.y - position.y - 1, position.x);
                case 180:
                    return new Vector2Int(dimensions.x - position.x - 1, dimensions.y - position.y - 1);
                case 270:
                    return new Vector2Int(position.y, dimensions.x - position.x - 1);
                default:
                    throw new ArgumentException("Invalid rotation angle. Only 90, 180, and 270 degrees are allowed.");
            }
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
                    mapping[Side.UP] = Side.RIGHT;
                    mapping[Side.RIGHT] = Side.DOWN;
                    mapping[Side.DOWN] = Side.LEFT;
                    mapping[Side.LEFT] = Side.UP;
                    mapping[Side.NONE] = Side.NONE;
                    break;
                case 180:
                    mapping[Side.UP] = Side.DOWN;
                    mapping[Side.RIGHT] = Side.LEFT;
                    mapping[Side.DOWN] = Side.UP;
                    mapping[Side.LEFT] = Side.RIGHT;
                    mapping[Side.NONE] = Side.NONE;
                    break;
                case 270:
                    mapping[Side.UP] = Side.LEFT;
                    mapping[Side.RIGHT] = Side.UP;
                    mapping[Side.DOWN] = Side.RIGHT;
                    mapping[Side.LEFT] = Side.DOWN;
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
                case Side.DOWN:
                    return Side.UP;
                case Side.UP:
                    return Side.DOWN;
                case Side.RIGHT:
                    return Side.LEFT;
                case Side.LEFT:
                    return Side.RIGHT;
                case Side.NONE:
                    return Side.NONE;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}