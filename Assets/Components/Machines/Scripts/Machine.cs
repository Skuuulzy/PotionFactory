using System;
using System.Collections.Generic;
using System.Linq;
using Components.Ingredients;
using Components.Machines.Behaviors;
using Components.Tick;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Components.Machines
{
    [Serializable]
    public class Machine : ITickable
    {
        // ----------------------------------------------------------------------- PRIVATE FIELDS -------------------------------------------------------------------------
        // TODO: Should be a dictionary ?
        [SerializeField] private List<IngredientTemplate> _inIngredients;
        [SerializeField] private List<IngredientTemplate> _outIngredients;
        [SerializeField] private List<Node> _nodes;
        [SerializeField, ReadOnly] private MachineController _controller;
        [SerializeField] private MachineBehavior _behavior;
        
        private readonly MachineTemplate _template;
        private int _outMachineTickCount;
        
        // ----------------------------------------------------------------------- PUBLIC FIELDS -------------------------------------------------------------------------
        public MachineTemplate Template => _template;
        public MachineController Controller => _controller;
        public MachineBehavior Behavior => _behavior;
        public List<IngredientTemplate> InIngredients => _inIngredients;
        public List<IngredientTemplate> OutIngredients => _outIngredients;
        public Dictionary<IngredientTemplate, int> GroupedInIngredients => _inIngredients.GroupedByTypeAndCount();
        public Dictionary<IngredientTemplate, int> GroupedOutIngredients => _outIngredients.GroupedByTypeAndCount();
        public virtual List<Node> Nodes => _nodes;

        // ------------------------------------------------------------------------- ACTIONS -------------------------------------------------------------------------
        public Action OnTick;
        public Action OnPropagateTick;
        public Action<bool> OnItemAdded;
        public static Action<Machine, bool> OnSelected;
        public static Action<Machine, bool> OnHovered;
        public static Action<Machine, bool> OnRetrieve;
        public static Action<Machine> OnMove;
        public static Action<Machine> OnConfigure;

        // TODO: This should not be here, some machine have specific event linked with specifics views.
        public Action OnItemSell;
        
        // --------------------------------------------------------------------- INITIALISATION -------------------------------------------------------------------------
        public Machine(MachineTemplate template, MachineController controller)
        {
            _template = template;
            _behavior = template.GetBehaviorClone();
            _controller = controller;

            UpdateNodesRotation(0);
            
            _inIngredients = new List<IngredientTemplate>();
            _outIngredients = new List<IngredientTemplate>();
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
        
        public bool TryGetOutMachines(out List<Machine> connectedMachines)
        {
            connectedMachines = new List<Machine>();
            
            foreach (var node in Nodes)
            {
                foreach (var port in node.Ports)
                {
                    if (port.Way != Way.OUT || port.ConnectedPort == null)
                    {
                        continue;
                    }

                    if (port.ConnectedPort.Node.Machine.Controller == null)
                    {
                        continue;
                    }
                    
                    // Get the other machine by the connected port.
                    connectedMachines.Add(port.ConnectedPort.Node.Machine);
                }
            }

            if (connectedMachines.Count > 0)
            {
                return true;
            }

            connectedMachines = null;
            return false;
        }
        
        public bool TryGetInMachine(out List<Machine> connectedMachines)
        {
            connectedMachines = new List<Machine>();
            
            foreach (var node in Nodes)
            {
                foreach (var port in node.Ports)
                {
                    if (port.Way != Way.IN || port.ConnectedPort == null)
                    {
                        continue;
                    }
                    
                    if (port.ConnectedPort.Node.Machine.Controller == null)
                    {
                        continue;
                    }
                    
                    // Get the other machine by the connected port.
                    connectedMachines.Add(port.ConnectedPort.Node.Machine);
                }
            }
            
            if (connectedMachines.Count > 0)
            {
                return true;
            }

            connectedMachines = null;
            return false;
        }

        // ------------------------------------------------------------------------- INGREDIENTS -------------------------------------------------------------------------
        /// Base input behaviour of a machine. Check if any room left if so store the item in the correct slot.
        public virtual bool TryInput(IngredientTemplate ingredient)
        {
            if (!CanAddIngredientOfTypeInSlot(ingredient, Way.IN))
            {
                return false;
            }

            AddIngredient(ingredient, Way.IN);
            
            return true;
        }
        
        public void AddIngredient(IngredientTemplate ingredient , Way way)
        {
            switch (way)
            {
                case Way.IN:
                    _inIngredients.Add(ingredient);
                    OnItemAdded?.Invoke(true);
                    break;
                case Way.OUT:
                    _outIngredients.Add(ingredient);
                    OnItemAdded?.Invoke(true);
                    break;
                case Way.NONE:
                    Debug.LogError("Way of adding item not handle.");
                    return;
            }
        }

        public IngredientTemplate TakeOlderIngredient()
        {
            if (_outIngredients.Count == 0)
            {
                return null;
            }

            var ingredient = _outIngredients.First();
            _outIngredients.Remove(ingredient);

            return ingredient;
        }
        
        public void RemoveAllItems()
        {
            InIngredients.Clear();
            OnItemAdded?.Invoke(false);
        }

        public void RemoveItem(int index)
        {
            _outIngredients.RemoveAt(index);
            OnItemAdded?.Invoke(false);
        }
        
        public void RemoveInItems(List<IngredientTemplate> ingredientToRemove)
        {
            _inIngredients.RemoveAll(item => ingredientToRemove.Any(b => b.Name == item.Name));            
            OnItemAdded?.Invoke(false);
        }
        
        public int EmptyInSlotCount()
        {
            int remainingSlot = Template.InSlotIngredientCount - _inIngredients.GroupedByTypeAndCount().Count;

            return remainingSlot < 0 ? 0 : remainingSlot;
        }
        
        public bool CanAddIngredientOfTypeInSlot(IngredientTemplate ingredient, Way way)
        {
            if (ingredient == null)
            {
                return false;
            }
            
            var ingredientsToLook = way == Way.IN ? _inIngredients : _outIngredients;
            var groupedIngredients = ingredientsToLook.GroupedByTypeAndCount();
            
            if (groupedIngredients.TryGetValue(ingredient, out var groupedIngredientCount))
            {
                if (groupedIngredientCount < Template.IngredientsPerSlotCount)
                {
                    return true;
                }

                return false;
            }
            
            return way == Way.IN ? EmptyInSlotCount() > 0 : _outIngredients.Count < Template.IngredientsPerSlotCount;
        }

        // ------------------------------------------------------------------------- TICK -------------------------------------------------------------------------
        public void Tick()
        {
            Behavior.Execute();
            
            // Propagate tick
            if (TryGetInMachine(out List<Machine> previousMachines))
            {
                foreach (var previousMachine in previousMachines)
                {
                    previousMachine.PropagateTick();
                }
            }
        }
        
        public void PropagateTick()
        {
            _outMachineTickCount++;

            if (!TryGetOutMachines(out var connectedMachines))
            {
                return;            
            }

            // The machine has not received the propagation of all his next machine.
            if (_outMachineTickCount < connectedMachines.Count)
            {
                return;
            }
            
            Behavior.Execute();

            // Propagate tick
            if (!TryGetInMachine(out List<Machine> previousMachines))
            {
                return;
            }
            
            foreach (var previousMachine in previousMachines)
            {
                previousMachine.PropagateTick();
            }

            _outMachineTickCount = 0;
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
        
        // ------------------------------------------------------------------------- SELECT BEHAVIOUR -------------------------------------------------------------------------
        public void Select(bool select)
        {
            OnSelected?.Invoke(this, select);
        }

        public void Hover(bool hovered)
        {
            OnHovered?.Invoke(this, hovered);
        }
    }
}