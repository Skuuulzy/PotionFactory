using System.Collections.Generic;

namespace Components.Machines.Behaviors
{
    public class SplitterMachineBehavior : MachineBehavior
    {
        private Machine _lastMachineGivenTo;

        public SplitterMachineBehavior(Machine machine) : base(machine) { }

        protected override Machine OutputMachine()
        {
            // If there is only one out machine, the behaviour do not change.
            if (Machine.TryGetOutMachines(out List<Machine> outMachines))
            {
                if (outMachines.Count == 1)
                {
                    return base.OutputMachine();
                }
            }

            if (_lastMachineGivenTo == null)
            {
                for (int i = 0; i < outMachines.Count; i++)
                {
                    var outMachine = outMachines[i];
                
                    if (outMachine.CanTakeIngredientInSlot(Machine.OlderOutIngredient(), Way.IN))
                    {
                        _lastMachineGivenTo = outMachine;
                        return outMachine;
                    }
                }
                
                return base.OutputMachine();
            }

            for (int i = 0; i < outMachines.Count; i++)
            {
                var outMachine = outMachines[i];

                if (outMachine == _lastMachineGivenTo)
                    continue;

                if (outMachine.CanTakeIngredientInSlot(Machine.OlderOutIngredient(), Way.IN))
                {
                    _lastMachineGivenTo = outMachine;
                    return outMachine;
                }
            }

            return base.OutputMachine();
        }
    }
}