using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameState : BaseState
{

	public static Action<EndGameState> OnEndGameStateStarted;
	public static Action<EndGameState> OnEndGameStateEnded;

	public override void SetName()
	{
		_stateName = "End Game state";
	}

	public override void OnEnter()
	{
		base.OnEnter();
		_isFinished = false;
		OnEndGameStateStarted?.Invoke(this);
	}

	public override void Update()
	{

	}

	public override void OnExit()
	{
		base.OnExit();
		_isFinished = true;
		OnEndGameStateEnded?.Invoke(this);
	}
}
