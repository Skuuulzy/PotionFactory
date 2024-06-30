using Components.Tick;
using UnityEngine;

namespace Components.Machines
{
    public class MachineController : MonoBehaviour, ITickable
    {
        [SerializeField] private Transform _3dViewHolder;

        private Machine _machine;

        public void Init(MachineTemplate machineTemplate)
        {
            _machine = new Machine(machineTemplate);
            
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