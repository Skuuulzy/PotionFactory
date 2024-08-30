using System;
using TMPro.EditorUtilities;
using UnityEngine;
using VComponent.Tools.Timer;

public class FactoryState : BaseState
{
	public static Action FactoryStateBegin;
	public static Action FactoryStateEnd;

	private CountdownTimer _countdownTimer;
	public override void SetName()
	{
		_stateName = "Factory state";
	}
	public override void OnEnter()
	{
		base.OnEnter();

		_isFinished = false;
		FactoryStateBegin?.Invoke();
		_countdownTimer = new CountdownTimer(10);
		_countdownTimer.OnTimerStop += SetStateFinished;
		_countdownTimer.Start();
		Debug.Log("Factory State begin");
	}

	public override void Update()
	{
		_countdownTimer.Tick(Time.deltaTime);
	}

	public override void OnExit()
	{
		FactoryStateEnd?.Invoke();
		Debug.Log("Factory State end");
	}

	private void SetStateFinished()
	{
		_isFinished = true;
	}

}

