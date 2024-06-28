using System;
using System.Collections.Generic;

namespace Components.Machines
{
    public abstract class Machine
    {
        public List<Side> InPorts { get; protected set; }
        public List<Side> OutPorts { get; protected set; }

        public List<Type> Items { get; protected set; }
        public int MaxItemCount { get; protected set; }

        public abstract void Tick();

        public abstract void ConstructFromTemplates(MachineTemplate template);

        public bool AcceptItem(Type item)
        {
            // There is already too many items in the machine
            if (Items.Count >= MaxItemCount)
                return false;
            
            Items.Add(item);
            return true;
        }
    }
}