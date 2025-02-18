using System;

public class PlanningFactoryState : BaseState
{
	public static Action<PlanningFactoryState> OnPlanningFactoryStateStarted;
	public static Action<PlanningFactoryState> OnPlanningFactoryStateEnded;

	public override void SetName()
	{
		_stateName = "Planning Factory state";
	}
	public override void OnEnter()
	{
		base.OnEnter();
		OnPlanningFactoryStateStarted?.Invoke(this);
		_isFinished = false;
	}


	public override void OnExit()
	{
		base.OnExit();
		OnPlanningFactoryStateEnded?.Invoke(this);
	}

}

