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
                if (connectedMachine.InPorts[0].Side != OutPorts[0].Side.Opposite())
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
                if (connectedMachine.OutPorts[0].Side != InPorts[0].Side.Opposite())
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
            
            _inPorts = _template.BaseInPorts.RotatePorts(angle, Template.Dimension);
            _outPorts = _template.BaseOutPorts.RotatePorts(angle, Template.Dimension);
        }
    }
}