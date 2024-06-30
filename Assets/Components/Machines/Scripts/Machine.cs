using System;
using System.Collections.Generic;

namespace Components.Machines
{
    public class Machine
    {
        private readonly MachineTemplate _template;
        public MachineTemplate Template => _template;
        
        public Machine InNeighbour;
        public Machine OutNeighbour;
        
        public List<Type> Items { get; }


        public Machine(MachineTemplate template)
        {
            _template = template;

            Items = new List<Type>();
        }
        
        public bool AcceptItem(Type item)
        {
            // There is already too many items in the machine
            if (Items.Count >= Template.MaxItemCount)
                return false;
            
            Items.Add(item);
            return true;
        }
    }
}