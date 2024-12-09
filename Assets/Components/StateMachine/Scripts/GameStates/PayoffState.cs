using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PayoffState : BaseState
{
	public static Action<PayoffState> OnPayoffStateStarted;
	public static Action<PayoffState> OnPayoffStateEnded;

	public override void SetName()
	{
		_stateName = "Shop state";
	}

	public override void OnEnter()
	{
		base.OnEnter();
		_isFinished = false;
		OnPayoffStateStarted?.Invoke(this);
	}

	public override void Update()
	{

	}

	public override void OnExit()
	{
		base.OnExit();
		_isFinished = true;
		OnPayoffStateEnded?.Invoke(this);
	}

	public void PayoffConfirm()
	{
		PayoffController.OnPayoffConfirm -= PayoffConfirm;
		base.SetStateFinished();
	}
}
