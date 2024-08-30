
using System;
using UnityEngine;
using VComponent.Tools.Timer;

public class ShopState : BaseState
{
	public static Action ShopStateBegin;
	public static Action ShopStateEnd;

	public override void SetName()
	{
		_stateName = "Shop state";
	}

	public override void OnEnter()
	{
		base.OnEnter();
		_isFinished = false;
		ShopStateBegin?.Invoke();
		Debug.Log("Shop State begin");
	}

	public override void Update()
	{

	}

	public override void OnExit()
	{
		_isFinished = true;
		ShopStateEnd?.Invoke();
		Debug.Log("Shop State end");
	}

}

