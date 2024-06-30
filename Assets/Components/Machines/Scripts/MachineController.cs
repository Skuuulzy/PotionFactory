using System.Collections.Generic;
using Components.Grid;
using Components.Tick;
using UnityEngine;

namespace Components.Machines
{
    public class MachineController : MonoBehaviour, ITickable
    {
        [SerializeField] private Transform _3dViewHolder;

        [SerializeField] private Machine _machine;


        public Machine Machine => _machine;

        public void Init(MachineTemplate machineTemplate, Dictionary<Side, Cell> neighbours)
        {
            _machine = new Machine(machineTemplate, neighbours);
            
            TickSystem.RegisterTickable(this);

            Instantiate(_machine.Template.View, _3dViewHolder);
        }

        private void OnDestroy()
        {
            TickSystem.UnregisterTickable(this);
        }

        public void Tick()
        {
            _machine.Template.Behavior.Process(_machine);
            
            //Debug.Log($"[MACHINES] Ticking {name} on frame {Time.frameCount}");
        }
    }
}