using System;
using Components.Bundle;
using Components.Economy;
using Components.Map;
using Cysharp.Threading.Tasks;
using SoWorkflow.SharedValues;
using UnityEngine;
using VComponent.Tools.Timer;
using static Components.GameParameters.GameParameters;

public class StateController : MonoBehaviour
{
	[SerializeField] private RunConfiguration _runConfiguration;
	[SerializeField] private bool _disable;
	[SerializeField] private SOSharedInt _dayIndex;

	public string CurrentDebugStateName;
	
	private Timer _timer;
	private StateMachine _stateMachine;

	[SerializeField] private SOSharedFloat _initialTime;
	[SerializeField] private SOSharedFloat _currentTime;

	public static Action<BaseState> OnStateStarted;
	
	
	//------------------------------------------------------------------------ MONO -------------------------------------------------------------------------------------------- 
	private void Start()
	{
		if (_disable)
		{
			return;
		}

		MapState.OnMapStateStarted += HandleMapState;
		BundleChoiceState.OnBundleStateStarted += HandleBundleChoiceState;
		PlanningFactoryState.OnPlanningFactoryStateStarted += HandlePlanningFactoryState;
		ResolutionFactoryState.OnResolutionFactoryStateStarted += HandleResolutionFactoryState;
		EndOfDayState.OnEndOfDayStateStarted += HandleShopState;
		EndGameState.OnEndGameStateStarted += HandleEndGameState;
		GameOverState.OnGameOverStarted += HandleGameOverState;

		//State Machine 
		_stateMachine = new StateMachine();

		//Declare state 
		BundleChoiceState bundleChoiceState = new BundleChoiceState();
		PlanningFactoryState planningFactoryState = new PlanningFactoryState();
		ResolutionFactoryState resolutionFactoryState = new ResolutionFactoryState();
		EndOfDayState endOfDayState = new EndOfDayState();
		EndGameState endGameState = new EndGameState();
		GameOverState gameOverState = new GameOverState();

		//Define transitions  
		At(bundleChoiceState, resolutionFactoryState, new FuncPredicate(() => bundleChoiceState.IsFinished));
		At(resolutionFactoryState, gameOverState, new FuncPredicate(() => resolutionFactoryState.IsFinished && EconomyController.Instance.CheckGameOver(resolutionFactoryState.StateIndex)));
		At(resolutionFactoryState, endOfDayState, new FuncPredicate(() => resolutionFactoryState.IsFinished && !EconomyController.Instance.CheckGameOver(resolutionFactoryState.StateIndex) && resolutionFactoryState.StateIndex < _runConfiguration.RunStateList.Count));
		At(resolutionFactoryState, endGameState, new FuncPredicate(() => resolutionFactoryState.IsFinished && !EconomyController.Instance.CheckGameOver(resolutionFactoryState.StateIndex) && resolutionFactoryState.StateIndex >= _runConfiguration.RunStateList.Count));
		At(endOfDayState, resolutionFactoryState, new FuncPredicate(() => endOfDayState.IsFinished));
		//At(endOfDayState, bundleChoiceState, new FuncPredicate(() => endOfDayState.IsFinished && endOfDayState.StateIndex % 4 == 0));

		StartStateMachine(bundleChoiceState);
	}
	
	private async void StartStateMachine(IState stateToStart)
	{
		//Wait 1sec to let everyone subscribe to event 
		await UniTask.WaitForSeconds(1);
		//Set initial state 
		_stateMachine.SetState(stateToStart);
	}

	private void OnDestroy()
	{
        MapState.OnMapStateStarted -= HandleMapState;
        BundleChoiceState.OnBundleStateStarted -= HandleBundleChoiceState;
        PlanningFactoryState.OnPlanningFactoryStateStarted -= HandlePlanningFactoryState;
        ResolutionFactoryState.OnResolutionFactoryStateStarted -= HandleResolutionFactoryState;
        EndOfDayState.OnEndOfDayStateStarted -= HandleShopState;
        EndGameState.OnEndGameStateStarted -= HandleEndGameState;
        GameOverState.OnGameOverStarted -= HandleGameOverState;
    }

	private void Update()
	{
		if (_disable)
		{
			return;
		}

		_stateMachine.Update();
		
		if (_timer != null)
		{
			_timer.Tick(Time.deltaTime);
			
			_initialTime.Set(_timer.InitialTime);
			_currentTime.Set(_timer.Time);
		}
	}

	//------------------------------------------------------------------------ STATE STARTED -------------------------------------------------------------------------------------------- 
	private void HandleMapState(MapState state)
	{
		CurrentDebugStateName = "MAP";
		
		MapGenerator.OnMapChoiceConfirm += state.MapChoiceConfirmed;
	}

	private void HandleBundleChoiceState(BundleChoiceState state)
	{
		CurrentDebugStateName = "BUNDLE CHOICE";

		BundleChoiceGenerator.OnBundleChoiceConfirm += state.BundleChoiceConfirmed;
	}

	private void HandleShopState(EndOfDayState state)
	{
		CurrentDebugStateName = "SHOP";
		_timer = null;
		OnStateStarted?.Invoke(state);
	}

	private void HandlePlanningFactoryState(PlanningFactoryState state)
	{
		CurrentDebugStateName = "PLANNING";

		if (CurrentGameMode == GameMode.STANDARD)
		{
			Debug.Log("Starting countdown");
			_timer = new CountdownTimer(_runConfiguration.PlanningFactoryStateTime);
			BaseState.OnStateEnded += _timer.Stop;
			_timer.OnTimerStop += state.SetStateFinished;
			_timer.Start();
		}
		else
		{
			Debug.Log("Starting stop watch");
			_timer = new StopwatchTimer();
			_timer.Start();
		}

		OnStateStarted.Invoke(state);
	}

	private void HandleResolutionFactoryState(ResolutionFactoryState state)
	{
		CurrentDebugStateName = "RESOLUTION";

		if (CurrentGameMode == GameMode.STANDARD)
		{
			_timer = new TickableCountdownTimer(_runConfiguration.GetStateTime(state.StateIndex));
			
			BaseState.OnStateEnded += _timer.Stop;
			_timer.OnTimerStop += state.SetStateFinished;
		}
		else
		{
			_timer = new TickableStopWatchTimer();
		}

		_dayIndex.Set(state.StateIndex);
		_initialTime.Set(_timer.InitialTime);
		_timer.Start();
		
		OnStateStarted.Invoke(state);
	}

	private void HandleEndGameState(EndGameState state)
	{
		CurrentDebugStateName = "END GAME";
		_timer = null;
		OnStateStarted?.Invoke(state);
	}

	private void HandleGameOverState(GameOverState state)
	{
		CurrentDebugStateName = "GAME OVER";
		_timer = null;
		OnStateStarted?.Invoke(state);
	}

	void At(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
	void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);
}