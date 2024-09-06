using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using VComponent.Tools.Timer;

public class StateController : MonoBehaviour
{
	[SerializeField] private UIStateController _uiStateController;
	[SerializeField] private int _planningFactoryStateTime = 180;
	[SerializeField] private int _resolutionFactoryStateTime = 120;
	
	private CountdownTimer _countdownTimer;

	// StateMachine
	private StateMachine _stateMachine;

	//------------------------------------------------------------------------ MONO --------------------------------------------------------------------------------------------
	void Start()
    {
        PlanningFactoryState.OnPlanningFactoryStateStarted += HandlePlanningFactoryState;
        ResolutionFactoryState.OnResolutionFactoryStateStarted += HandleResolutionFactoryState;
        ShopState.OnShopStateStarted += HandleShopState;

		//State Machine
		_stateMachine = new StateMachine();

		//Declare state
		PlanningFactoryState planningFactoryState = new PlanningFactoryState();
		ResolutionFactoryState resolutionFactoryState = new ResolutionFactoryState();
		ShopState shopState = new ShopState();

		//Define transitions 
		At(planningFactoryState, resolutionFactoryState, new FuncPredicate(() => planningFactoryState.IsFinished));
		At(resolutionFactoryState, shopState, new FuncPredicate(() => resolutionFactoryState.IsFinished));
		At(shopState, planningFactoryState, new FuncPredicate(() => shopState.IsFinished));

		StartStateMachine(planningFactoryState);
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

	void Update()
    {
		_stateMachine.Update();
		if (_countdownTimer != null )
		{
			_countdownTimer.Tick(Time.deltaTime);
			_uiStateController.SetCountdownTime(_countdownTimer.Time, _countdownTimer.InitialTime);
		}
    }

	//------------------------------------------------------------------------ STATE STARTED --------------------------------------------------------------------------------------------

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
}
