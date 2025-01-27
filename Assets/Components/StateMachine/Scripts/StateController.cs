using System;
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
		PlanningFactoryState.OnPlanningFactoryStateStarted += HandlePlanningFactoryState;
		ResolutionFactoryState.OnResolutionFactoryStateStarted += HandleResolutionFactoryState;
		ShopState.OnShopStateStarted += HandleShopState;
		EndGameState.OnEndGameStateStarted += HandleEndGameState;

		//State Machine 
		_stateMachine = new StateMachine();

		//Declare state 
		MapState mapState = new MapState();
		PlanningFactoryState planningFactoryState = new PlanningFactoryState();
		ResolutionFactoryState resolutionFactoryState = new ResolutionFactoryState();
		ShopState shopState = new ShopState();
		EndGameState endGameState = new EndGameState();

		//Define transitions  
		At(mapState, planningFactoryState, new FuncPredicate(() => mapState.IsFinished));
		At(planningFactoryState, resolutionFactoryState, new FuncPredicate(() => planningFactoryState.IsFinished));
		At(resolutionFactoryState, shopState, new FuncPredicate(() => resolutionFactoryState.IsFinished && resolutionFactoryState.StateIndex < _runConfiguration.RunStateList.Count));
		At(resolutionFactoryState, endGameState, new FuncPredicate(() => resolutionFactoryState.IsFinished && resolutionFactoryState.StateIndex >= _runConfiguration.RunStateList.Count));
		At(shopState, planningFactoryState, new FuncPredicate(() => shopState.IsFinished && shopState.StateIndex % 4 != 0));
		At(shopState, mapState, new FuncPredicate(() => shopState.IsFinished && shopState.StateIndex % 4 == 0));

		StartStateMachine(mapState);
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
		ShopState.OnShopStateStarted -= HandleShopState;
		EndGameState.OnEndGameStateStarted -= HandleEndGameState;

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

	private void HandleShopState(ShopState state)
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
		
		_countdownTimer = new CountdownTimer(_runConfiguration.ResolutionFactoryStateTime);
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

	void At(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
	void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);
}