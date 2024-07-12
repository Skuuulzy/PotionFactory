using System.Collections.Generic;
using Components.Grid;
using Components.Items;
using Components.Tick;
using UnityEngine;

namespace Components.Machines
{
    public class MachineController : MonoBehaviour
    {
        [SerializeField] private Transform _3dViewHolder;
        [SerializeField] private Machine _machine;
        [SerializeField] private ItemController _itemController;

        private bool _initialized;

        public Machine Machine => _machine;

        public void SetGridData(MachineTemplate machineTemplate, Dictionary<Side, Cell> neighbours, int rotation)
        {
            _initialized = true;
            
            _machine = new Machine(machineTemplate, neighbours, rotation, this);
            _machine.OnTick += Tick;
            _machine.OnItemAdded += ShowDebugItem;

            AddMachineToChain();
            Instantiate(_machine.Template.GridView, _3dViewHolder);
        }

        private void OnDestroy()
        {
            if (!_initialized)
            {
                return;
            }
            
            RemoveMachineFromChain();
            
            _machine.OnTick -= Tick;
            _machine.OnItemAdded -= ShowDebugItem;
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

        // ------------------------------------------------------------------------- DEBUG -------------------------------------------------------------------------

        private void ShowDebugItem(bool show)
        {
            if (show)
            {
                _itemController.Init(_machine.Items[0].Resources, _machine.Items[0].Types);
            }
            else
            {
                _itemController.DestructItem();
            }
		}
    }
}