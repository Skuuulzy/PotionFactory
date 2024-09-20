using UnityEngine;

namespace Components.Machines.Behaviors
{
    public abstract class MachineBehavior : ScriptableObject
    {
        [SerializeField] protected int _processTime;
        
        protected int CurrentTick;

        public int ProcessTime => _processTime;
        public bool ProcessingRecipe { get; protected set; }

        public abstract void Process(Machine machine);

        protected virtual bool CanProcess(int currentTick)
        {
            return CurrentTick % _processTime == 0;
        }
        
        public MachineBehavior Clone()
        {
            return Instantiate(this);
        }

        public void ChangeProcessTime(float percentage)
        {
            _processTime += Mathf.RoundToInt( _processTime * percentage);
        }
    }
}