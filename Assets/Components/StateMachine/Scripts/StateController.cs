using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using VComponent.Tools.Timer;

public class StateController : MonoBehaviour
{
	[SerializeField] private UIStateController _uiStateController;
	[SerializeField] private int _factoryStateTime = 15;
	
	private CountdownTimer _countdownTimer;

	// StateMachine
	private StateMachine _stateMachine;

	//------------------------------------------------------------------------ MONO --------------------------------------------------------------------------------------------
	void Start()
    {
        FactoryState.OnFactoryStateStarted += FactoryStateStarted;
        ShopState.OnShopStateStarted += ShopStateStarted;

		//State Machine
		_stateMachine = new StateMachine();

		//Declare state
		FactoryState factoryState = new FactoryState();
		ShopState shopState = new ShopState();

		//Define transitions 
		At(factoryState, shopState, new FuncPredicate(() => factoryState.IsFinished));
		At(shopState, factoryState, new FuncPredicate(() => shopState.IsFinished));

		//Set initial state
		_stateMachine.SetState(factoryState);
	}

	private void OnDestroy()
	{
		FactoryState.OnFactoryStateStarted -= FactoryStateStarted;
		ShopState.OnShopStateStarted -= ShopStateStarted;
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

	private void ShopStateStarted(ShopState state)
	{
		_uiStateController.DisplayFinishShopButton(state);
	}

	private void FactoryStateStarted(FactoryState state)
	{
		_countdownTimer = new CountdownTimer(_factoryStateTime);
		_countdownTimer.OnTimerStop += state.SetStateFinished;
		_countdownTimer.OnTimerStop += _uiStateController.HideCountdown;
		_countdownTimer.Start();
	}


	void At(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
	void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);
}
