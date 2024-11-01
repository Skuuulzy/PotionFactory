using Components.Bundle;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapState : BaseState
{
	public static Action<MapState> OnMapStateStarted;
	public static Action<MapState> OnMapStateEnded;

	public override void SetName()
	{
		_stateName = "Planning Factory state";
	}
	public override void OnEnter()
	{
		base.OnEnter();
		OnMapStateStarted?.Invoke(this);
		_isFinished = false;
	}


	public override void OnExit()
	{
		base.OnExit();
		OnMapStateEnded?.Invoke(this);
	}

	public void MapChoiceConfirmed(IngredientsBundle bundle, bool isFirstChoice)
	{
		base.SetStateFinished();
	}
}
