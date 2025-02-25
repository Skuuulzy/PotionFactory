using System;
using System.Collections.Generic;
using Components.Machines;
using Sirenix.OdinInspector;
using SoWorkflow.SharedValues;
using UnityEngine;
using VComponent.Tools.Singletons;
using VTools.SoWorkflow.EventSystem;
using static UnityEngine.Rendering.DebugUI;

namespace Components.Tick
{
    public class TickSystem : Singleton<TickSystem>
    {
        [SerializeField] private float _initialTickDuration = 0.2f;
        [ShowInInspector] private static readonly List<ITickable> TICKABLES = new();
        
        private float _tickTimer;

        [SerializeField] private SOSharedFloat _currentTickDuration;
        [SerializeField] private SOSharedInt _currentTickMultiplier;
		private bool _isPause;

        private static bool _log;

        public float InitialTickDuration => _initialTickDuration;
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
            _currentTickDuration.Set(_initialTickDuration);
            PlanningFactoryState.OnPlanningFactoryStateStarted += HandlePlanningFactoryState;
            ResolutionFactoryState.OnResolutionFactoryStateStarted += HandleResolutionFactoryState;
            EndOfDayState.OnEndOfDayStateStarted += HandleEndOfDayState;
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
            
            while (_tickTimer >= _currentTickDuration.Value)
            {
                _tickTimer -= _currentTickDuration.Value;
                
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

            if (_log && tickable is Machine m1)
            {
                Debug.Log($"[TICK SYSTEM] Adding : {m1.Controller.name} to tickables");
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
            
            if (_log && previousTickable is Machine m1 && newTickable is Machine m2)
            {
                Debug.Log($"[TICK SYSTEM] Replacing : {m1.Controller.name} by {m2.Controller.name} from tickables");
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

            if (_log && tickableToRemove is Machine machine)
            {
                Debug.Log($"[TICK SYSTEM] Removing : {machine.Controller.name} from tickables");
            }
            
            TICKABLES.Remove(tickableToRemove);
        }

        public void ChangeTimeSpeed(int value)
        {
            _currentTickMultiplier.Set(value);

            if (value == 0)
			{
                _isPause = true;
                return;
			}

            _currentTickDuration.Set(_initialTickDuration / value);
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
        
        // ------------------------------------------------------------------------- DEBUG -------------------------------------------------------------------------

        [Button(ButtonSizes.Small)]
        public void ToggleLogs()
        {
            _log = !_log;
        }
    }
}