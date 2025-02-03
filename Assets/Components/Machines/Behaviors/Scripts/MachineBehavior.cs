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
        
        // ------------------------------------------------------------------------- PROCESS LOOP -------------------------------------------------------------------------
        
        /// Optional pre-process, happen before the process, not affected by the process time of the machine, happen at each tick.
        protected virtual void PreProcess() { }
        
        /// Base process of the machine:
        /// 1. Apply pre-process.
        /// 2. Check if the machine can process based on the tick.
        /// 3. Apply machine process behaviour.
        public void Process()
        {
            PreProcess();
            
            // Check if we can process
            CurrentProcessTime++;
            
            if (CurrentProcessTime < ProcessTime)
            {
                return;
            }
            CurrentProcessTime = 0;
            
            SubProcess();
        }

        /// Base sub process for all machine, transfer ingredient from in to out slot, FIFO.
        /// Override for specific process.
        protected virtual void SubProcess()
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

        // ------------------------------------------------------------------------- IN/OUT -------------------------------------------------------------------------

        /// Base input behaviour of a machine. Check if any room left if so store the item in the correct slot.
        public virtual bool TryInput(IngredientTemplate ingredient)
        {
            if (!Machine.CanAddIngredientOfTypeInSlot(ingredient, Way.IN))
            {
                return false;
            }

            Machine.AddIngredient(ingredient, Way.IN);
            
            return true;
        }
        
        /// Base out behaviour of a machine. Check if any item to give and if any valid receiver.
        public virtual bool TryOutput()
        {
            if (Machine.OutIngredients.Count <= 0)
            {
                return false;
            }

            if (!Machine.TryGetOutMachines(out List<Machine> outMachines))
            {
                return false;
            }

            // By default, a machine should only have one input, otherwise override this method.
            if (!outMachines[0].Behavior.TryInput(Machine.OutIngredients[0]))
            {
                return false;
            }
            
            Machine.RemoveItem(0);
            return true;
        }
        
        // ------------------------------------------------------------------------- RELICS -------------------------------------------------------------------------
        public void AddRelicEffect(RelicEffect effect)
        {
            
        }
    }
}