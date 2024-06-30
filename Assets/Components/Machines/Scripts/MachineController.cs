using System.Collections.Generic;
using Components.Grid;
using Components.Tick;
using UnityEngine;

namespace Components.Machines
{
    public class MachineController : MonoBehaviour
    {
        [SerializeField] private Transform _3dViewHolder;
        [SerializeField] private Machine _machine;
        [SerializeField] private GameObject _debugItem;
        
        public Machine Machine => _machine;

        public void Init(MachineTemplate machineTemplate, Dictionary<Side, Cell> neighbours)
        {
            _machine = new Machine(machineTemplate, neighbours);
            _machine.OnTick += Tick;
            _machine.OnItemAdded += ShowDebugItem;

            TryUpdateLastChainElement();

            Instantiate(_machine.Template.View, _3dViewHolder);
        }

        private void OnDestroy()
        {
            _machine.OnTick -= Tick;
            _machine.OnItemAdded -= ShowDebugItem;
        }

        public void Tick()
        {
            _machine.Template.Behavior.Process(_machine);
            
            // Propagate tick
            if (_machine.TryGetInMachine(out Machine previousMachine))
            {
                previousMachine.Tick();
            }
        }
        
        private void TryUpdateLastChainElement()
        {
            // The machine is not connected to any chain, create a new one.
            if (!_machine.TryGetInMachine(out _) && !_machine.TryGetOutMachine(out _))
            {
                TickSystem.RegisterAsNewEndChainElement(_machine);
            }

            // The machine is placed at the end of the chain, the previous element should be the last element register.
            // We need to update the chain
            if (_machine.TryGetInMachine(out Machine previousMachine) && !_machine.TryGetOutMachine(out _))
            {
                TickSystem.ReplaceEndChainElement(previousMachine, _machine);
            }
        }
        
        private void ShowDebugItem(bool show)
        {
            _debugItem.SetActive(show);
        }
    }
}