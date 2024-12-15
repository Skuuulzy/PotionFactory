using System.Collections.Generic;
using Components.Ingredients;
using UnityEngine;

namespace Components.Machines.Behaviors
{
    public abstract class MachineBehavior : ScriptableObject
    {
        private int _initialProcessTime;
        protected int CurrentTick;

        public float RelicEffectBonusProcessTime = 0;
        public List<RelicEffect> RelicEffects;
        
        protected int InitialProcessTime => _initialProcessTime;

        public bool ProcessingRecipe { get; protected set; }

        public abstract void Process(Machine machine);
        public abstract void TryGiveOutIngredient(Machine machine);

        public virtual bool CanTakeItem(Machine machine, Machine fromMachine, IngredientTemplate ingredient)
        {
            if (machine.Template.CanTakeInfiniteIngredients)
            {
                return true;
            }

            return machine.CanAddIngredientOfTypeInSlot(ingredient, Way.IN);
        }

        public void SetInitialProcessTime(int processTime)
        {
            _initialProcessTime = processTime;
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