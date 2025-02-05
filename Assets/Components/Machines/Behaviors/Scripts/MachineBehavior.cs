using Components.Ingredients;

namespace Components.Machines.Behaviors
{
    public class MachineBehavior
    {
        protected readonly Machine Machine;
        private readonly int _baseProcessTime;
        protected int AdditionalProcessTime;
        
        public int CurrentProcessTime { get; private set; }
        public int ProcessTime => _baseProcessTime + AdditionalProcessTime;
        
        public MachineBehavior(Machine machine)
        {
            Machine = machine;
            _baseProcessTime = machine.Template.ProcessTime;
        }
        
        // ------------------------------------------------------------------------- EXECUTION LOOP -------------------------------------------------------------------------

        /// Base behavior to determine if a machine can receive an ingredient.
        protected virtual bool CanReceive(IngredientTemplate ingredientToInput)
        {
            return Machine.CanTakeIngredientInSlot(ingredientToInput, Way.IN);
        }
        
        /// Main execution loop of the machine:
        /// 1. Apply pre-process.
        /// 2. Apply Process.
        /// 3. Output the result.
        public void Execute()
        {
            PreProcess();
            Process();
            Output();
        }
        
        /// Optional pre-process, happen before the process, not affected by the process time of the machine, happen at each tick.
        protected virtual void PreProcess() { }
        
        private void Process()
        {
            // Check if we can process
            CurrentProcessTime++;
            
            if (CurrentProcessTime < ProcessTime)
            {
                return;
            }
            CurrentProcessTime = 0;
            
            ProcessAction();
        }

        /// Base process action for all machine, transfer ingredient from IN to OUT slot, FIFO. Override for specific process.
        protected virtual void ProcessAction()
        {
            if (Machine.InIngredients.Count <= 0)
            {
                return;
            }

            var ingredientToMove = Machine.InIngredients[0];
            
            // Is there any space left in the out slot.
            if (Machine.CanTakeIngredientInSlot(ingredientToMove, Way.OUT))
            {
                Machine.AddIngredient(ingredientToMove, Way.OUT);
                Machine.RemoveIngredient(ingredientToMove, Way.IN);
                Machine.OnProcess?.Invoke();
            }
        }

        /// Base out behaviour of a machine. Check if any item to give and if any valid receiver.
        private void Output()
        {
            if (Machine.OutIngredients.Count <= 0)
            {
                return;
            }

            var machineToOutput = OutputMachine();
            if (machineToOutput == null)
            {
                return;
            }
            
            var ingredientToOutput = Machine.OlderOutIngredient();
            if (!ingredientToOutput)
            {
                return;
            }

            if (!machineToOutput.Behavior.CanReceive(ingredientToOutput))
            {
                return;
            }

            machineToOutput.AddIngredient(ingredientToOutput, Way.IN);
            Machine.RemoveIngredient(ingredientToOutput, Way.OUT);
        }

        protected virtual Machine OutputMachine()
        {
            return !Machine.TryGetOutMachines(out var outMachines) ? null : outMachines[0];
        }
        
        // ------------------------------------------------------------------------- RELICS -------------------------------------------------------------------------
        public void AddRelicEffect(RelicEffect effect)
        {
            
        }
    }
}