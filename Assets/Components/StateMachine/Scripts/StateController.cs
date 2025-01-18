using System;
using Components.Map;
using Components.Shop.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VComponent.Tools.Timer;

public class StateController : MonoBehaviour
{
	[SerializeField] private bool _disable;
	[SerializeField] private int _planningFactoryStateTime = 180;
	[SerializeField] private int _resolutionFactoryStateTime = 120;

	[SerializeField] private string _currentDebugStateName;
	
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

		//State Machine 
		_stateMachine = new StateMachine();

		//Declare state 
		MapState mapState = new MapState();
		PlanningFactoryState planningFactoryState = new PlanningFactoryState();
		ResolutionFactoryState resolutionFactoryState = new ResolutionFactoryState();
		ShopState shopState = new ShopState();

		//Define transitions  
		At(mapState, planningFactoryState, new FuncPredicate(() => mapState.IsFinished));
		At(planningFactoryState, resolutionFactoryState, new FuncPredicate(() => planningFactoryState.IsFinished));
		At(resolutionFactoryState, shopState, new FuncPredicate(() => resolutionFactoryState.IsFinished));
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
		_currentDebugStateName = "MAP";
		
		MapGenerator.OnMapChoiceConfirm += state.MapChoiceConfirmed;
	}

	private void HandleShopState(ShopState state)
	{
		_currentDebugStateName = "SHOP";
		_countdownTimer = null;
		OnStateStarted?.Invoke(state);
	}

	private void HandlePlanningFactoryState(PlanningFactoryState state)
	{
		_currentDebugStateName = "PLANNING";
		
		_countdownTimer = new CountdownTimer(_planningFactoryStateTime);
		BaseState.OnStateEnded += _countdownTimer.Stop;
		_countdownTimer.OnTimerStop += state.SetStateFinished;
		_countdownTimer.Start();

		OnStateStarted.Invoke(state);
	}

	private void HandleResolutionFactoryState(ResolutionFactoryState state)
	{
		_currentDebugStateName = "RESOLUTION";
		
		_countdownTimer = new CountdownTimer(_resolutionFactoryStateTime);
		BaseState.OnStateEnded += _countdownTimer.Stop;
		_countdownTimer.OnTimerStop += state.SetStateFinished;
		_countdownTimer.Start();
		
		OnStateStarted.Invoke(state);
	}

	void At(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
	void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);
}