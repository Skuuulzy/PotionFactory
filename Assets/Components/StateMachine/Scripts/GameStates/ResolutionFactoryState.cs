using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionFactoryState : BaseState
{
	public static Action<ResolutionFactoryState> OnResolutionFactoryStateStarted;
	public static Action<ResolutionFactoryState> OnResolutionFactoryStateEnded;

	public override void SetName()
	{
		_stateName = "Resolution Factory state";
	}
	public override void OnEnter()
	{
		base.OnEnter();
		OnResolutionFactoryStateStarted?.Invoke(this);
		_isFinished = false;
	}


	public override void OnExit()
	{
		base.OnExit();
		OnResolutionFactoryStateEnded?.Invoke(this);
	}
}
