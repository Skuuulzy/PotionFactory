using System;
using Components.Interactions.Clickable;
using Components.Items;
using Components.Tick;
using UnityEngine;

namespace Components.Machines
{
    public class MachineController : MonoBehaviour, IClickable
    {
        [SerializeField] private Transform _3dViewHolder;
        [SerializeField] private Machine _machine;
        [SerializeField] private ItemController _itemController;
        [SerializeField] private GameObject _debugItem;

        public static Action<Machine> OnMachineClicked;
        
        public Machine Machine => _machine;

        private bool _initialized;
        private GameObject _view;
        
        // ------------------------------------------------------------------------- INIT -------------------------------------------------------------------------
        public void InstantiatePreview(MachineTemplate machineTemplate, float scale)
        {
            _view = Instantiate(machineTemplate.GridView, _3dViewHolder);
            _machine = new Machine(machineTemplate, this);
            _view.transform.localScale = new Vector3(scale, scale, scale);
        }
        
        public void RotatePreview(int angle)
        {
            _view.transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
            _machine.UpdateNodesRotation(angle);
        }
        
        public void ConfirmPlacement()
        {
            _initialized = true;
            
            _machine.OnTick += Tick;
            _machine.OnItemAdded += ShowItem;
            _machine.LinkNodeData();
            
            AddMachineToChain();
        }

        private void OnDestroy()
        {
            if (!_initialized)
            {
                return;
            }
            
            RemoveMachineFromChain();
            
            _machine.OnTick -= Tick;
            _machine.OnItemAdded -= ShowItem;
        }

        private void Tick()
        {
            _machine.Behavior.Process(_machine);
            
            // Propagate tick
            if (_machine.TryGetInMachine(out Machine previousMachine))
            {
                previousMachine.Tick();
            }
        }

        // ------------------------------------------------------------------------- CHAIN -------------------------------------------------------------------------
        private void AddMachineToChain()
        {
            bool hasInMachine = _machine.TryGetInMachine(out Machine inMachine);
            bool hasOutMachine = _machine.TryGetOutMachine(out _);

            // The machine is not connected to any chain, create a new one.
            if (!hasInMachine && !hasOutMachine)
            {
                TickSystem.AddTickable(_machine);
            }
            // The machine only has an IN, it is now the end of the chain.
            if (hasInMachine && !hasOutMachine)
            {
                TickSystem.ReplaceTickable(inMachine, _machine);
            }
            // The machine has an IN and an OUT, it makes a link between two existing chains,
            // remove the IN tickable since the out chain already has a tickable.
            if (hasInMachine && hasOutMachine)
            {
                TickSystem.RemoveTickable(inMachine);
            }
        }

        private void RemoveMachineFromChain()
        {
            bool hasInMachine = _machine.TryGetInMachine(out Machine inMachine);
            bool hasOutMachine = _machine.TryGetOutMachine(out _);
            
            // The machine is not connected to any chain, create a new one.
            if (!hasInMachine && !hasOutMachine)
            {
                TickSystem.RemoveTickable(_machine);
            }
            // The machine only has an IN, it is now the end of the chain.
            if (hasInMachine && !hasOutMachine)
            {
                TickSystem.ReplaceTickable(_machine, inMachine);
            }
            // The machine has an IN and an OUT, it makes a link between two existing chains,
            // remove the IN tickable since the out chain already has a tickable.
            if (hasInMachine && hasOutMachine)
            {
                TickSystem.AddTickable(inMachine);
            }
        }

        // ------------------------------------------------------------------------- ITEM -------------------------------------------------------------------------
        private void ShowItem(bool show)
        {
            _debugItem.SetActive(show);
            return;
            
            if (show)
            {
                _itemController.CreateRepresentationWith(_machine.Ingredients[0].Resources, _machine.Ingredients[0].Types);
            }
            else
            {
                _itemController.DestroyRepresentation();
            }
        }
        
        // ------------------------------------------------------------------------- CLICKABLE BEHAVIOUR -------------------------------------------------------------------------
        public void Clicked()
        {
            if (!_initialized)
            {
                return;
            }
            
            OnMachineClicked?.Invoke(_machine);
        }
    }
}