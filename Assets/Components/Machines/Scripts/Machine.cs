using System;
using System.Collections.Generic;
using Components.Items;
using Components.Machines.Behaviors;
using Components.Tick;
using UnityEngine;

namespace Components.Machines
{
    [Serializable]
    public class Machine : ITickable
    {
        // ----------------------------------------------------------------------- PRIVATE FIELDS -------------------------------------------------------------------------
        [SerializeField] private List<IngredientTemplate> _ingredients;
        [SerializeField] private List<Node> _nodes;
        [SerializeField] private MachineController _controller;
        [SerializeField] private MachineBehavior _behavior;
        
        private readonly MachineTemplate _template;
        
        // ----------------------------------------------------------------------- PUBLIC FIELDS -------------------------------------------------------------------------
        public MachineTemplate Template => _template;
        public MachineController Controller => _controller;
        public MachineBehavior Behavior => _behavior;
        public List<IngredientTemplate> Ingredients => _ingredients;
        public virtual List<Node> Nodes => _nodes;
        
        // ------------------------------------------------------------------------- ACTIONS -------------------------------------------------------------------------
        public Action OnTick;
        public Action<bool> OnItemAdded;
        
        // --------------------------------------------------------------------- INITIALISATION -------------------------------------------------------------------------
        public Machine(MachineTemplate template, MachineController controller)
        {
            _template = template;
            _behavior = template.GetBehaviorClone();
            _controller = controller;

            UpdateNodesRotation(0);
            
            _ingredients = new List<IngredientTemplate>();
        }

        public void UpdateNodesRotation(int rotation)
        {
            ReplaceNodesViaRotation(rotation, true);
        }

        public void SetNodeData()
        {
            
        }
        
        // ------------------------------------------------------------------------- NODES -------------------------------------------------------------------------
        public void LinkNodeData()
        {
            foreach (var node in Nodes)
            {
                node.SetMachine(this);
            }
        }
        
        public bool TryGetOutMachine(out Machine connectedMachine)
        {
            foreach (var node in Nodes)
            {
                foreach (var port in node.Ports)
                {
                    if (port.Way != Way.OUT || port.ConnectedPort == null)
                    {
                        continue;
                    }
                    
                    // Get the other machine by the connected port.
                    connectedMachine = port.ConnectedPort.Node.Machine;
                    return true;
                }
            }

            connectedMachine = null;
            return false;
        }
        
        public bool TryGetInMachine(out Machine connectedMachine)
        {
            foreach (var node in Nodes)
            {
                foreach (var port in node.Ports)
                {
                    if (port.Way != Way.IN || port.ConnectedPort == null)
                    {
                        continue;
                    }
                    
                    // Get the other machine by the connected port.
                    connectedMachine = port.ConnectedPort.Node.Machine;
                    return true;
                }
            }

            connectedMachine = null;
            return false;
        }

        // ------------------------------------------------------------------------- ITEMS -------------------------------------------------------------------------
        public void AddItem()
        {
            OnItemAdded?.Invoke(true);
        }
        
        public bool TryGiveItemItem(IngredientTemplate ingredient)
        {
            // There is already too many items in the machine
            if (Template.MaxItemCount != -1 && Ingredients.Count >= Template.MaxItemCount)
                return false;

            if (Behavior.ProcessingRecipe)
            {
                return false;
            }
            
            Ingredients.Add(ingredient);
            OnItemAdded?.Invoke(true);
            return true;
        }

        public void RemoveAllItems()
        {
            Ingredients.Clear();
            OnItemAdded?.Invoke(false);
        }

        public void RemoveItem(int index)
        {
            Ingredients.RemoveAt(index);
            OnItemAdded?.Invoke(false);
        }
        
        public void ClearItems()
        {
            Ingredients.Clear();
            OnItemAdded?.Invoke(false);
        }

        // ------------------------------------------------------------------------- TICK -------------------------------------------------------------------------
        public void Tick()
        {
            OnTick?.Invoke();
        }
        
        // ------------------------------------------------------------------- PORTS & ROTATIONS -------------------------------------------------------------------------
        public void ReplaceNodesViaRotation(int angle, bool clockwise)
        {
            if (!clockwise)
            {
                angle = 360 - angle;
            }

            angle %= 360;

            // The machine has no rotation so the ports are the base one.
            if (angle == 0)
            {
                _nodes = _template.Nodes;
                
                return;
            }

            _nodes = Template.Nodes.RotateNodes(angle);
        }
    }
}