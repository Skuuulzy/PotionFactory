using System.Collections.Generic;
using Components.Ingredients;
using Components.Tick;
using UnityEngine;

namespace Components.Machines.Behaviors
{
    public abstract class MachineBehavior : ScriptableObject
    {
        protected Machine Machine;

        private int _baseProcessTime;
        protected int AdditionalProcessTime;
        
        public int CurrentProcessTime { get; private set; }
        public int ProcessTime => _baseProcessTime + AdditionalProcessTime;
        
        public void Initialize(Machine machine)
        {
            _baseProcessTime = machine.Template.ProcessTime;
            Machine = machine;
        }
        
        public MachineBehavior Clone()
        {
            return Instantiate(this);
        }
        
        // ------------------------------------------------------------------------- EXECUTION LOOP -------------------------------------------------------------------------
        
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

        /// Base sub process for all machine, transfer ingredient from in to out slot, FIFO. Override for specific process.
        protected virtual void ProcessAction()
        {
            if (Machine.InIngredients.Count <= 0)
            {
                return;
            }
            
            // Is there any space left in the out slot.
            if (Machine.CanAddIngredientOfTypeInSlot(Machine.InIngredients[0], Way.OUT))
            {
                // Add the ingredient to the machine out slot.
                Machine.AddIngredient(Machine.InIngredients[0], Way.OUT);
                // TODO: Remove ingredient from in
            }
        }

        /// Base out behaviour of a machine. Check if any item to give and if any valid receiver.
        protected virtual void Output()
        {
            if (Machine.OutIngredients.Count <= 0)
            {
                return;
            }

            if (!Machine.TryGetOutMachines(out List<Machine> outMachines))
            {
                return;
            }

            // By default, a machine should only have one input, otherwise override this method.
            if (!outMachines[0].TryInput(Machine.OutIngredients[0]))
            {
                return;
            }
            
            Machine.RemoveItem(0);
        }
        
        // ------------------------------------------------------------------------- RELICS -------------------------------------------------------------------------
        public void AddRelicEffect(RelicEffect effect)
        {
            
        }
    }
}