using Components.Bundle;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BundleChoiceState : BaseState
{
	public static Action<BundleChoiceState> OnBundleStateStarted;
	public static Action<BundleChoiceState> OnBundleStateEnded;

	public override void SetName()
	{
		_stateName = "Bundle Choice state";
	}
	public override void OnEnter()
	{
		base.OnEnter();
		OnBundleStateStarted?.Invoke(this);
		_isFinished = false;
	}


	public override void OnExit()
	{
		base.OnExit();
		OnBundleStateEnded?.Invoke(this);
	}

	public void BundleChoiceConfirmed(IngredientsBundle bundle, bool isFirstChoice)
	{
		base.SetStateFinished();
	}
}
