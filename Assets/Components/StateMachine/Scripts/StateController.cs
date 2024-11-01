using Components.Map;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using VComponent.Tools.Timer;

public class StateController : MonoBehaviour
{
	[SerializeField] private bool _disable;
	[SerializeField] private UIStateController _uiStateController;
	[SerializeField] private int _planningFactoryStateTime = 180;
	[SerializeField] private int _resolutionFactoryStateTime = 120;

	private CountdownTimer _countdownTimer;

	// StateMachine 
	private StateMachine _stateMachine;

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
		At(shopState, planningFactoryState, new FuncPredicate(() => shopState.IsFinished));

		StartStateMachine(mapState);
	}



	private async void StartStateMachine(BaseState stateToStart)
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
			_uiStateController.SetCountdownTime(_countdownTimer.Time, _countdownTimer.InitialTime);
		}
	}

	//------------------------------------------------------------------------ STATE STARTED -------------------------------------------------------------------------------------------- 
	private void HandleMapState(MapState state)
	{
		MapGenerator.OnMapChoiceConfirm += state.MapChoiceConfirmed;
	}

	private void HandleShopState(ShopState state)
	{
		_uiStateController.DisplayFinishStateButton(state);
		_countdownTimer = null;
	}

	private void HandlePlanningFactoryState(PlanningFactoryState state)
	{
		_countdownTimer = new CountdownTimer(_planningFactoryStateTime);
		BaseState.OnStateEnded += _countdownTimer.Stop;
		_countdownTimer.OnTimerStop += state.SetStateFinished;
		_countdownTimer.OnTimerStop += _uiStateController.HideCountdown;
		_countdownTimer.Start();
		_uiStateController.DisplayFinishStateButton(state);

	}

	private void HandleResolutionFactoryState(ResolutionFactoryState state)
	{
		_countdownTimer = new CountdownTimer(_resolutionFactoryStateTime);
		BaseState.OnStateEnded += _countdownTimer.Stop;
		_countdownTimer.OnTimerStop += state.SetStateFinished;
		_countdownTimer.OnTimerStop += _uiStateController.HideCountdown;
		_countdownTimer.Start();
		_uiStateController.DisplayFinishStateButton(state);
	}

	void At(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
	void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);

	public void LaunchMainMenu()
	{
		SceneManager.LoadScene("Main_Menu", LoadSceneMode.Single);
	}
}