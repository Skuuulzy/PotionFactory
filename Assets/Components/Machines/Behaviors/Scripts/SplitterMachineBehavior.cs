using System.Collections.Generic;

namespace Components.Machines.Behaviors
{
    public class SplitterMachineBehavior : MachineBehavior
    {
        private int _outputIndex;

        public SplitterMachineBehavior(Machine machine) : base(machine) { }

        protected override Machine OutputMachine()
        {
            // If there is only one out machine, the behaviour do not change.
            if (!Machine.TryGetOutMachines(out List<Machine> outMachines))
            {
                return null;
            }
            
            if (outMachines.Count == 1)
            {
                return base.OutputMachine();
            }

            // Cycle through potential out machines index to dispatch resources.
            _outputIndex = (_outputIndex + 1) % outMachines.Count;
            
            return outMachines[_outputIndex];
        }
    }
}