using System;
using TMPro.EditorUtilities;
using UnityEngine;
using VComponent.Tools.Timer;

public class FactoryState : BaseState
{
	public static Action<FactoryState> OnFactoryStateStarted;
	public static Action<FactoryState> OnFactoryStateEnded;

	public override void SetName()
	{
		_stateName = "Factory state";
	}
	public override void OnEnter()
	{
		base.OnEnter();
		OnFactoryStateStarted?.Invoke(this);
		_isFinished = false;
	}


	public override void OnExit()
	{
		base.OnExit();
		OnFactoryStateEnded?.Invoke(this);
	}

}

