using System;

public class GameOverState : BaseState
{
	public static Action<GameOverState> OnGameOverStarted;
	public static Action<GameOverState> OnGameOverEnded;

	public override void SetName()
	{
		_stateName = "GameOver state";
	}

	public override void OnEnter()
	{
		base.OnEnter();
		_isFinished = false;
		OnGameOverStarted?.Invoke(this);
	}

	public override void Update()
	{

	}

	public override void OnExit()
	{
		base.OnExit();
		_isFinished = true;
		OnGameOverEnded?.Invoke(this);
	}
}
