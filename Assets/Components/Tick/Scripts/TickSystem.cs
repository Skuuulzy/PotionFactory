using System.Collections.Generic;
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

        [SerializeField] private SerializableDictionary<int, TextMeshProUGUI> _timerTextList;
        
        private static readonly List<ITickable> TICKABLES = new();

        public float TickDuration => _tickDuration;

        private void Update()
        {
            _tickTimer += Time.deltaTime;
            
            while (_tickTimer >= _tickDuration)
            {
                _tickTimer -= _tickDuration;
                _tick++;
                DisplayTime();
                
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
        
        public static void RegisterAsNewEndChainElement(ITickable tickable)
        {
            TICKABLES.Add(tickable);
        }

        public static void RegisterTickable(ITickable tickable)
        {
            if (!TICKABLES.Contains(tickable))
            {
                TICKABLES.Insert(0, tickable);
            }
        }

        public static void UnregisterTickable(ITickable tickable)
        {
            if (TICKABLES.Contains(tickable))
            {
                TICKABLES.Remove(tickable);
            }
        }

        private void DisplayTime()
        {
            _timerTextList[1].text = "1 Tick = " + _tick;

            if (_tick % 5 == 0)
            {
                if (_timerTextList.ContainsKey(5))
                {
                    _timerTextList[5].text = "5 Tick = " + _tick / 5;
                }
            }

            if (_tick % 10 == 0)
            {
                if (_timerTextList.ContainsKey(10))
                {
                    _timerTextList[10].text = "10 Tick = " + _tick / 10;
                }
            }

            if (_tick % 20 == 0)
            {
                if (_timerTextList.ContainsKey(20))
                {
                    _timerTextList[20].text = "20 Tick = " + _tick / 20;
                }
            }
        }

        public void ChangeTimeSpeed(int value)
        {
            Time.timeScale = value;
        }

        public static void ReplaceEndChainElement(ITickable previousTickable, ITickable newTickable)
        {
            if (!TICKABLES.Contains(previousTickable))
            {
                Debug.LogError($"You try to replace a tickable but it was not found in the tickable list.");
                return;
            }

            int previousTickableIndex = TICKABLES.IndexOf(previousTickable);
            TICKABLES[previousTickableIndex] = newTickable;
        }
    }
}