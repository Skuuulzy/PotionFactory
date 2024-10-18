using System;

public abstract partial class BaseState : IState
{
	protected bool _isFinished;
	public bool IsFinished => _isFinished;

	public static Action<BaseState> OnStateStarted;
	public static Action OnStateEnded;
	protected string _stateName;
	private int _stateIndex;

	public string StateName => _stateName;
	public int StateIndex => _stateIndex;

	public virtual void SetName()
	{
		_stateName = "Base state";
	}

	public virtual void OnEnter()
	{
		SetName();
		_stateIndex++;
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