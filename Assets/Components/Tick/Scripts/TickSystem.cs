using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using VComponent.Tools.Singletons;

namespace Components.Tick
{
    public class TickSystem : Singleton<TickSystem>
    {
        private float _tickTimer;
        private int _tick;
        [SerializeField] private float _tickDuration = 0.2f;
        
        [ShowInInspector]
        private static readonly List<ITickable> TICKABLES = new();

        public float TickDuration => _tickDuration;

        private void Update()
        {
            _tickTimer += Time.deltaTime;
            
            while (_tickTimer >= _tickDuration)
            {
                _tickTimer -= _tickDuration;
                _tick++;
                
                TickAll();
            }
        }
        
        private void TickAll()
        {
            foreach (var tickable in TICKABLES)
            {
                tickable.Tick();
            }
        }
        
        public static void AddTickable(ITickable tickable)
        {
            TICKABLES.Add(tickable);
        }
        
        public static void ReplaceTickable(ITickable previousTickable, ITickable newTickable)
        {
            if (!TICKABLES.Contains(previousTickable))
            {
                Debug.LogError($"You try to replace a tickable but it was not found in the tickable list.");
                return;
            }

            int previousTickableIndex = TICKABLES.IndexOf(previousTickable);
            TICKABLES[previousTickableIndex] = newTickable;
        }
        
        public static void RemoveTickable(ITickable tickableToRemove)
        {
            if (!TICKABLES.Contains(tickableToRemove))
            {
                Debug.LogError($"You try to remove a tickable but it was not found in the tickable list.");
                return;
            }

            TICKABLES.Remove(tickableToRemove);
        }

        public void ChangeTimeSpeed(int value)
        {
            Time.timeScale = value;
        }
    }
}