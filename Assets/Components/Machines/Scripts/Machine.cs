using System;
using System.Collections.Generic;
using Components.Tick;

namespace Components.Machines
{
    public class Machine : ITickable
    {
        private readonly MachineType _type;
        private readonly List<Side> _inPorts;
        private readonly List<Side> _outPorts;
        private readonly int _maxItemCount;
        private readonly MachineTemplate _template;
        
        public List<Type> Items { get; }
        public MachineTemplate Template => _template;
        public MachineType Type => _type;

        public Machine(MachineTemplate template)
        {
            _template = template;
            
            _type = template.Type;
            _inPorts = template.InPorts;
            _outPorts = template.OutPorts;
            _maxItemCount = template.MaxItemCount;

            Items = new List<Type>();
            
            TickSystem.RegisterTickable(this);
        }

        ~Machine()
        {
            TickSystem.UnregisterTickable(this);
        }

        public virtual void Tick()
        {
            
        }
        
        public bool AcceptItem(Type item)
        {
            // There is already too many items in the machine
            if (Items.Count >= _maxItemCount)
                return false;
            
            Items.Add(item);
            return true;
        }
    }
}