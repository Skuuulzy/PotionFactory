
using System;
using UnityEngine;
using VComponent.Tools.Timer;

public class ShopState : BaseState
{
	public static Action<ShopState> OnShopStateStarted;
	public static Action<ShopState> OnShopStateEnded;

	public override void SetName()
	{
		_stateName = "Shop state";
	}

	public override void OnEnter()
	{
		base.OnEnter();
		_isFinished = false;
		OnShopStateStarted?.Invoke(this);
	}

	public override void Update()
	{

	}

	public override void OnExit()
	{
		base.OnExit();
		_isFinished = true;
		OnShopStateEnded?.Invoke(this);
	}

}

