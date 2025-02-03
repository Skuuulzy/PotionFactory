using System.Collections.Generic;
using Components.Ingredients;
using UnityEngine;

namespace Components.Machines.Behaviors
{
    public abstract class MachineBehavior : ScriptableObject
    {
        protected Machine Machine;
        protected int _initialProcessTime;
        protected int CurrentTick;

        public float RelicEffectBonusProcessTime;
        public List<RelicEffect> RelicEffects;
        
        public bool ProcessingRecipe { get; protected set; }

        public void Initialize(Machine machine)
        {
            _initialProcessTime = machine.Template.ProcessTime;
            Machine = machine;
        }

        public abstract void Process();
        
        public abstract void TryGiveOutIngredient();

        public virtual bool CanTakeItem(Machine machine, Machine fromMachine, IngredientTemplate ingredient)
        {
            if (machine.Template.CanTakeInfiniteIngredients)
            {
                return true;
            }

            return machine.CanAddIngredientOfTypeInSlot(ingredient, Way.IN);
        }

        protected virtual bool CanProcess(int currentTick)
        {
            return CurrentTick % _initialProcessTime == 0;
        }
        
        public MachineBehavior Clone()
        {
            return Instantiate(this);
        }
    }
}