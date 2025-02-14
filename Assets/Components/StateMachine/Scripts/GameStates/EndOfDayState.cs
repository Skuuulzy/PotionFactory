
using System;
using UnityEngine;
using VComponent.Tools.Timer;

public class EndOfDayState : BaseState
{
	public static Action<EndOfDayState> OnEndOfDayStateStarted;
	public static Action<EndOfDayState> OnEndOfDayStateEnded;

	public override void SetName()
	{
		_stateName = "End of day state";
	}

	public override void OnEnter()
	{
		base.OnEnter();
		_isFinished = false;
		OnEndOfDayStateStarted?.Invoke(this);
	}

	public override void Update()
	{

	}

	public override void OnExit()
	{
		base.OnExit();
		_isFinished = true;
		OnEndOfDayStateEnded?.Invoke(this);
	}

}

