using System.Collections.Generic;
using Components.Ingredients;
using Components.Tick;
using UnityEngine;
using Components.Machines.Behaviors;

namespace Components.Machines
{
    public partial class MachineController : MonoBehaviour
    {
        [SerializeField] private Transform _3dViewHolder;
        [SerializeField] private Machine _machine;
        [SerializeField] private IngredientController _ingredientController;
        
        private GameObject _view;
        
        private bool _initialized;
        private bool _selected;

        
        public Machine Machine => _machine;
        
        // ------------------------------------------------------------------------- INIT -----------------------------------------------------------------------------
        public void InstantiatePreview(MachineTemplate machineTemplate, float scale, bool showOutlines = false)
        {
            _view = Instantiate(machineTemplate.GridView, _3dViewHolder);
            _machine = new Machine(machineTemplate, this);
            _view.transform.localScale = new Vector3(scale, scale, scale);

            SetupDirectionalArrows(machineTemplate);

            _outline = _view.AddComponent<Outline>();
            _outline.OutlineWidth = 8;
            ToggleOutlines(showOutlines, _placableColor);
        }
        
        public void RotatePreview(int angle)
        {
            _view.transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
            _machine.UpdateNodesRotation(angle);
        }
        
        public void ConfirmPlacement()
        {
            _machine.OnItemAdded += ShowItem;
            _machine.OnItemSell += ShowSellFeedback;
            Machine.OnSelected += HandleMachineSelected;
            Machine.OnHovered += HandleMachineHovered;
            
            _initialized = true;
            _machine.Behavior.Initialize(_machine);
            _machine.LinkNodeData();

            if(_machine.Behavior is DestructorMachineBehaviour destructor)
            {
                destructor.OnSpecialIngredientChanged += ShowItem;
            }

            AddMachineToChain();
            
            ToggleDirectionalArrows(false);
            ToggleOutlines(false);
        }

        // ------------------------------------------------------------------------- DESTROY --------------------------------------------------------------------------
        private void OnDestroy()
        {
            _machine.OnItemAdded -= ShowItem;
            _machine.OnItemSell -= ShowSellFeedback;
            Machine.OnSelected -= HandleMachineSelected;
            Machine.OnHovered -= HandleMachineHovered;
            
            if (!_initialized)
            {
                return;
            }
            
            RemoveMachineFromChain();
        }

        // ------------------------------------------------------------------------- CHAIN ----------------------------------------------------------------------------
        private void AddMachineToChain()
        {
            bool hasInMachine = _machine.TryGetInMachine(out List<Machine> inMachines);
            bool hasOutMachine = _machine.TryGetOutMachines(out _);

            // The machine is not connected to any chain, create a new one.
            if (!hasInMachine && !hasOutMachine)
            {
                TickSystem.AddTickable(_machine);
            }
            // The machine only has an IN, it is now the end of the chain.
            if (hasInMachine && !hasOutMachine)
            {
                foreach (var inMachine in inMachines)
                {
                    TickSystem.ReplaceTickable(inMachine, _machine);
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

        private void RemoveMachineFromChain()
        {
            bool hasInMachine = _machine.TryGetInMachine(out List<Machine> inMachines);
            bool hasOutMachine = _machine.TryGetOutMachines(out _);
            
            // The machine is not connected to any chain, create a new one.
            if (!hasInMachine && !hasOutMachine)
            {
                TickSystem.RemoveTickable(_machine);
            }
            // The machine only has an IN, it is now the end of the chain.
            if (hasInMachine && !hasOutMachine)
            {
                foreach (var inMachine in inMachines)
                {
                    TickSystem.ReplaceTickable(_machine, inMachine);
                }
            }
            // The machine has an IN and an OUT, it makes a link between two existing chains,
            // remove the IN tickable since the out chain already has a tickable.
            if (hasInMachine && hasOutMachine)
            {
                foreach (var inMachine in inMachines)
                {
                    TickSystem.AddTickable(inMachine);
                }
            }
        }

        // ------------------------------------------------------------------------- CONTEXTUAL ACTIONS ---------------------------------------------------------------
        public void Move()
        {
            Machine.OnMove?.Invoke(Machine);
        }

        public void Configure()
        {
            Machine.OnConfigure?.Invoke(Machine);
        }

        public void Retrieve()
        {
            Machine.OnRetrieve?.Invoke(Machine, true);
        }
        
        // ------------------------------------------------------------------------- HANDLERS -------------------------------------------------------------------------
        private void HandleMachineSelected(Machine machine, bool selected)
        {
            if (Machine != machine)
            {
                ToggleDirectionalArrows(false);
                ToggleOutlines(false);
                _selected = false;
                
                return;
            }

            _selected = true;
            ToggleDirectionalArrows(selected);
            ToggleOutlines(selected, _selectedColor);
        }
        
        private void HandleMachineHovered(Machine machine, bool hovered)
        {
            if (_selected)
            {
                return;
            }
            
            if (Machine != machine)
            {
                ToggleOutlines(false);
                return;
            }
            
            ToggleOutlines(hovered, _hoveredColor);

        }
    }
}