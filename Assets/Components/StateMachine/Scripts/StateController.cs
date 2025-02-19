using System;
using Components.Bundle;
using Components.Economy;
using Components.Map;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VComponent.Tools.Timer;

public class StateController : MonoBehaviour
{
	[SerializeField] private RunConfiguration _runConfiguration;
	[SerializeField] private bool _disable;


	public string CurrentDebugStateName;
	
	private CountdownTimer _countdownTimer;
	private StateMachine _stateMachine;

	public static Action<float, float> OnCountdown;
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
		PlanningFactoryState.OnPlanningFactoryStateStarted -= HandlePlanningFactoryState;
		ResolutionFactoryState.OnResolutionFactoryStateStarted -= HandleResolutionFactoryState;
		EndOfDayState.OnEndOfDayStateStarted -= HandleShopState;
		EndGameState.OnEndGameStateStarted -= HandleEndGameState;
		BundleChoiceState.OnBundleStateStarted -= HandleBundleChoiceState;
	}

	private void Update()
	{
		if (_disable)
		{
			return;
		}

		_stateMachine.Update();
		if (_countdownTimer != null)
		{
			_countdownTimer.Tick(Time.deltaTime);
			OnCountdown?.Invoke(_countdownTimer.Time, _countdownTimer.InitialTime);
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
		_countdownTimer = null;
		OnStateStarted?.Invoke(state);
	}

	private void HandlePlanningFactoryState(PlanningFactoryState state)
	{
		CurrentDebugStateName = "PLANNING";
		
		_countdownTimer = new CountdownTimer(_runConfiguration.PlanningFactoryStateTime);
		BaseState.OnStateEnded += _countdownTimer.Stop;
		_countdownTimer.OnTimerStop += state.SetStateFinished;
		_countdownTimer.Start();

		OnStateStarted.Invoke(state);
	}

	private void HandleResolutionFactoryState(ResolutionFactoryState state)
	{
		CurrentDebugStateName = "RESOLUTION";
		
		_countdownTimer = new TickableCountdownTimer(_runConfiguration.GetStateTime(state.StateIndex));
		BaseState.OnStateEnded += _countdownTimer.Stop;
		_countdownTimer.OnTimerStop += state.SetStateFinished;
		_countdownTimer.Start();
		
		OnStateStarted.Invoke(state);
	}

	private void HandleEndGameState(EndGameState state)
	{
		CurrentDebugStateName = "END GAME";
		_countdownTimer = null;
		OnStateStarted?.Invoke(state);
	}

	private void HandleGameOverState(GameOverState state)
	{
		CurrentDebugStateName = "GAME OVER";
		_countdownTimer = null;
		OnStateStarted?.Invoke(state);
	}

	void At(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
	void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);
}