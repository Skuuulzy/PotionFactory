using UnityEngine;

namespace Components.Machines.Behaviors
{
    public abstract class MachineBehavior : ScriptableObject
    {
        private int _initialProcessTime;
        protected int CurrentTick;

        protected int InitialProcessTime => _initialProcessTime;

        public bool ProcessingRecipe { get; protected set; }

        public abstract void Process(Machine machine);

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