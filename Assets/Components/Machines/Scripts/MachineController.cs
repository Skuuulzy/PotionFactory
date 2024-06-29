using System;
using Components.Tick;
using UnityEngine;

namespace Components.Machines
{
    public class MachineController : MonoBehaviour, ITickable

    {
        [SerializeField] private Transform _3dViewHolder;

        private Machine _machine;

        public void Init(Machine machine)
        {
            _machine = machine;
            TickSystem.RegisterTickable(this);

            Instantiate(machine.Template.View, _3dViewHolder);
        }

        private void OnDestroy()
        {
            TickSystem.UnregisterTickable(this);
        }

        public void Tick()
        {
            Debug.Log($"[MACHINES] Ticking {name} on frame {Time.frameCount}");
        }
    }
}