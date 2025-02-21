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
        
        private MachineBehavior _behavior;
        private readonly MachineTemplate _template;
        private int _outMachineTickCount;
        
        // ----------------------------------------------------------------------- PUBLIC FIELDS -------------------------------------------------------------------------
        public MachineTemplate Template => _template;
        public MachineController Controller => _controller;
        public MachineBehavior Behavior => _behavior;
        public List<IngredientTemplate> InIngredients => _inIngredients;
        public List<IngredientTemplate> OutIngredients => _outIngredients;
        public List<IngredientTemplate> AllIngredients => _inIngredients.Concat(_outIngredients).ToList();
        public virtual List<Node> Nodes => _nodes;

        // ------------------------------------------------------------------------- ACTIONS -------------------------------------------------------------------------
        public Action OnTick;
        public Action OnPropagateTick;
        public Action OnSlotUpdated;
        public static Action<Machine, bool> OnSelected;
        public static Action<Machine, bool> OnHovered;
        public static Action<Machine, bool> OnRetrieve;
        public static Action<Machine> OnMove;
        public static Action<Machine> OnConfigure;
        public static Action<Machine, bool> OnProcess;

        // TODO: This should not be here, some machine have specific event linked with specifics views.
        public Action OnItemSell;
        public int Rotation { get; private set; }
        
        // --------------------------------------------------------------------- INITIALISATION -------------------------------------------------------------------------
        
        public Machine(MachineTemplate template, MachineController controller)
        {
            _template = template;
            _behavior = GetBehavior(template.Type);
            _controller = controller;

            UpdateNodesRotation(0);
            
            _inIngredients = new List<IngredientTemplate>();
            _outIngredients = new List<IngredientTemplate>();
        }

        private MachineBehavior GetBehavior(MachineType type)
        {
            switch (type)
            {
                case MachineType.CAULDRON:
                case MachineType.DISTILLER:
                case MachineType.MIXER:
                case MachineType.PRESS:
                    return new RecipeCreationBehavior(this);
                case MachineType.CONVEYOR:
                    return new ConveyorMachineBehavior(this);
                case MachineType.MARCHAND:
                    return new MarchandMachineBehaviour(this);
                case MachineType.EXTRACTOR:
                    return new ExtractorMachineBehaviour(this);
                case MachineType.MERGER:
                    return new MergerMachineBehavior(this);
                case MachineType.SPLITTER:
                    return new SplitterMachineBehavior(this);
                default:
                    Debug.LogError($"Unknown behaviour linked to type: {type}. Default behaviour used.");
                    return new MachineBehavior(this);
            }
        }

        public void UpdateNodesRotation(int rotation)
        {
            ReplaceNodesViaRotation(rotation, true);
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

        // ------------------------------------------------------------------------- TICK CHAIN ----------------------------------------------------------------------------
        public void AddMachineToChain()
        {
            bool hasInMachine = TryGetInMachine(out List<Machine> inMachines);
            bool hasOutMachine = TryGetOutMachines(out _);

            // The machine is not connected to any chain, create a new one.
            if (!hasInMachine && !hasOutMachine)
            {
                TickSystem.AddTickable(this);
            }
            // The machine only has an IN, it is now the end of the chain.
            if (hasInMachine && !hasOutMachine)
            {
                foreach (var inMachine in inMachines)
                {
                    TickSystem.ReplaceTickable(inMachine, this);
                }
            }
            // The machine has an IN and an OUT, it makes a link between two existing chains,
            // remove the IN tickable since the out chain already has a tickable.
            if (hasInMachine && hasOutMachine)
            {
                foreach (var inMachine in inMachines)
                {
                    TickSystem.RemoveTickable(inMachine);
                }
            }
        }

        public void RemoveMachineFromChain()
        {
            bool hasInMachine = TryGetInMachine(out List<Machine> inMachines);
            bool hasOutMachine = TryGetOutMachines(out _);
            
            // The machine is not connected to any chain, create a new one.
            if (!hasInMachine && !hasOutMachine)
            {
                TickSystem.RemoveTickable(this);
            }
            // The machine only has an IN, it is now the end of the chain.
            if (hasInMachine && !hasOutMachine)
            {
                foreach (var inMachine in inMachines)
                {
                    TickSystem.ReplaceTickable(this, inMachine);
                }
            }
            // The machine has an IN and an OUT, it breaks a chain in two.
            // Add the IN tickable since to create the second chain.
            if (hasInMachine && hasOutMachine)
            {
                foreach (var inMachine in inMachines)
                {
                    // If the machine has multiple out machines it is already part of a tick chain.
                    if (inMachine.TryGetOutMachines(out var outMachines))
                    {
                        if (outMachines.Count > 1)
                        {
                            continue;
                        }
                    }
                    
                    TickSystem.AddTickable(inMachine);
                }
            }
        }
        
        // ------------------------------------------------------------------------- INGREDIENTS -------------------------------------------------------------------------
        
        public void AddIngredient(IngredientTemplate ingredient , Way way)
        {
            switch (way)
            {
                case Way.IN:
                    _inIngredients.Add(ingredient);
                    break;
                case Way.OUT:
                    _outIngredients.Add(ingredient);
                    break;
                case Way.NONE:
                    Debug.LogError("Way of adding item not handle.");
                    return;
            }
            
            OnSlotUpdated?.Invoke();
        }
        
        public void RemoveIngredient(IngredientTemplate ingredientToRemove, Way slotType)
        {
            switch (slotType)
            {
                case Way.IN:
                    if (_inIngredients.Contains(ingredientToRemove))
                    {
                        _inIngredients.Remove(ingredientToRemove);
                    }
                    else
                    {
                        Debug.LogError($"Cannot remove ingredient: {ingredientToRemove.Name} from out slot of {_controller.name} because it cannot be found."); 
                    }
                    break;
                case Way.OUT:
                    if (_outIngredients.Contains(ingredientToRemove))
                    {
                        _outIngredients.Remove(ingredientToRemove);
                    }
                    else
                    {
                        Debug.LogError($"Cannot remove ingredient: {ingredientToRemove.Name} from out slot of {_controller.name} because it cannot be found."); 
                    }
                    break;
                case Way.NONE:
                    Debug.LogError("Cannot remove an ingredient if the slot type is not defined."); 
                    return;
            }
            
            OnSlotUpdated?.Invoke();
        }

        public void ClearSlot(Way slotType)
        {
            if (slotType == Way.IN)
            {
                _inIngredients.Clear();
            }
            else
            {
                _outIngredients.Clear();
            }
            
            OnSlotUpdated?.Invoke();
        }
        
        public bool CanTakeIngredientInSlot(IngredientTemplate ingredient, Way way)
        {
            if (!ingredient)
            {
                return false;
            }

            if (way == Way.OUT)
            {
                if (_outIngredients.Count == 0)
                {
                    return true;
                }
                if (_outIngredients.Count >= Template.IngredientsPerSlotCount)
                {
                    return false;
                }
                
                return _outIngredients[0].Name == ingredient.Name;
            }
            
            var groupedIngredients = _inIngredients.GroupedByTypeAndCount();

            // If the ingredient is not stored in any slot we check if there is an empty in slot.
            if (!groupedIngredients.TryGetValue(ingredient, out var groupedIngredientCount))
            {
                return EmptyInSlotCount() > 0;
            }
            
            // If the ingredient is already stored we check if the slot is not full yet.
            return groupedIngredientCount < Template.IngredientsPerSlotCount;
        }
        
        public IngredientTemplate OlderOutIngredient()
        {
            if (_outIngredients.Count == 0)
            {
                return null;
            }

            var ingredient = _outIngredients.First();
            return ingredient;
        }
        
        public int EmptyInSlotCount()
        {
            int remainingSlot = Template.InSlotIngredientCount - _inIngredients.GroupedByTypeAndCount().Count;
            return remainingSlot < 0 ? 0 : remainingSlot;
        }

        // ------------------------------------------------------------------------- TICK -------------------------------------------------------------------------
        
        public void Tick()
        {
            Behavior.Execute();
            OnTick?.Invoke();
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
            if (!TryGetOutMachines(out var connectedMachines))
            {
                return;            
            }

            _outMachineTickCount++;
            
            // The machine has not received the propagation of all his next machine.
            if (_outMachineTickCount < connectedMachines.Count)
            {
                return;
            }
            
            _outMachineTickCount = 0;
            
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
            Rotation = angle;
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