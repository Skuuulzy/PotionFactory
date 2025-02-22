using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using VComponent.Tools.Singletons;
using static UnityEngine.Rendering.DebugUI;

namespace Components.Tick
{
    public class TickSystem : Singleton<TickSystem>
    {
        [SerializeField] private float _initialTickDuration = 0.2f;
        [ShowInInspector] private static readonly List<ITickable> TICKABLES = new();
        
        private float _tickTimer;
        private float _currentTickDuration;
		private bool _isPause;

        public float InitialTickDuration => _initialTickDuration;
        public float CurrentTickDuration => _currentTickDuration;

        public static Action<int> OnTickMultiplierChanged;
        // ------------------------------------------------------------------------- MONO -------------------------------------------------------------------------
		protected override void Awake()
        {
            base.Awake();
            
            if (TICKABLES.Count == 0)
            {
                return;
            }
            
            TICKABLES.Clear();
        }

		private void Start()
		{
            _currentTickDuration = _initialTickDuration;
            PlanningFactoryState.OnPlanningFactoryStateStarted += HandlePlanningFactoryState;
            ResolutionFactoryState.OnResolutionFactoryStateStarted += HandleResolutionFactoryState;
            EndOfDayState.OnEndOfDayStateStarted += HandleEndOfDayState;
            UIOptionsController.OnTickSpeedUpdated += ChangeTimeSpeed;
            GameOverState.OnGameOverStarted += HandleGameOverState;
        }

		private void OnDestroy()
        {
            PlanningFactoryState.OnPlanningFactoryStateStarted -= HandlePlanningFactoryState;
            ResolutionFactoryState.OnResolutionFactoryStateStarted -= HandleResolutionFactoryState;
            EndOfDayState.OnEndOfDayStateStarted -= HandleEndOfDayState;
            GameOverState.OnGameOverStarted -= HandleGameOverState;
        }

        private void Update()
        {
            if(_isPause)
			{
                return;
			}

            _tickTimer += Time.deltaTime;
            
            while (_tickTimer >= _currentTickDuration)
            {
                _tickTimer -= _currentTickDuration;
                
                TickAll();
            }
        }

        // ------------------------------------------------------------------------- TICK METHODS -------------------------------------------------------------------------

        private void TickAll()
        {
            foreach (var tickable in TICKABLES)
            {
                tickable.Tick();
            }
        }
        
        public static void AddTickable(ITickable tickable)
        {
            if (TICKABLES.Contains(tickable))
            {
                return;
            }
            
            TICKABLES.Add(tickable);
        }
        
        public static void ReplaceTickable(ITickable previousTickable, ITickable newTickable)
        {
            // This happens when a machine has multiple outputs, it replaced when his first out is connected.
            // But then his next outputs need to be also added has a new partial chain.
            if (!TICKABLES.Contains(previousTickable))
            {
                AddTickable(newTickable);
                return;
            }

            int previousTickableIndex = TICKABLES.IndexOf(previousTickable);
            TICKABLES[previousTickableIndex] = newTickable;
        }
        
        public static void RemoveTickable(ITickable tickableToRemove)
        {
            if (!TICKABLES.Contains(tickableToRemove))
            {
                return;
            }

            TICKABLES.Remove(tickableToRemove);
        }

        public void ChangeTimeSpeed(int value)
        {
            OnTickMultiplierChanged?.Invoke(value);
            if (value == 0)
			{
                _isPause = true;
                return;
			}

            _currentTickDuration =  _initialTickDuration / value;
            _isPause = false;
        }

        // ------------------------------------------------------------------------- STATE METHODS -------------------------------------------------------------------------

        private void HandleEndOfDayState(EndOfDayState obj)
        {
            ChangeTimeSpeed(0);
        }

        private void HandlePlanningFactoryState(PlanningFactoryState obj)
        {
            ChangeTimeSpeed(0);
        }

        private void HandleGameOverState(GameOverState obj)
		{
            ChangeTimeSpeed(0);
		}

        private void HandleResolutionFactoryState(ResolutionFactoryState obj)
        {
            ChangeTimeSpeed(1);
        }

        // ------------------------------------------------------------------------- CONVERT TIME IN TICK -------------------------------------------------------------------------
        public static int GetTickValueFromSeconds(float value)
        {
            return Mathf.RoundToInt(value / TickSystem.Instance.InitialTickDuration);
        }
        public static int GetSecondValueFromTicks(int tick)
        {
            return Mathf.RoundToInt(tick * TickSystem.Instance.InitialTickDuration);
        }
    }
}