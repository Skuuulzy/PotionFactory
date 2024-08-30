using System;
using static UnityEngine.CullingGroup;
using System.Security.Cryptography.X509Certificates;

public abstract partial class BaseState : IState
{
	protected bool _isFinished;
	public bool IsFinished => _isFinished;

	public static Action<BaseState> OnStateStarted;
	public static Action OnStateEnded;
	protected string _stateName;

	public string StateName => _stateName;

	public virtual void SetName()
	{
		_stateName = "Base state";
	}

	public virtual void OnEnter()
	{
		SetName();
		OnStateStarted?.Invoke(this);
	}

	public virtual void Update()
	{

		//noop
	}

	public virtual void FixedUpdate()
	{

		//noop
	}

	public virtual void OnExit()
	{
		OnStateEnded?.Invoke();
	}

	public virtual void SetStateFinished()
	{
		_isFinished = true;
	}
}
