using System;
using UnityEngine;

public class UIOptionsController : MonoBehaviour
{
    public static Action<int> OnTickSpeedUpdated;
    public static Action OnClearGrid;

    private bool _canUpdateTickSpeed;

	private void Start()
	{
        BaseState.OnStateStarted += HandleAnyStateStarted;
	}
	private void OnDestroy()
	{
        BaseState.OnStateStarted -= HandleAnyStateStarted;
    }

    private void HandleAnyStateStarted(BaseState state)
	{
        //Tick speed can only change when the state is the resolution state, otherwise the tick speed need to stay at 0
        _canUpdateTickSpeed = state is ResolutionFactoryState;
	}

    public void ChangeTimeSpeed(int value)
    {
		if (!_canUpdateTickSpeed)
		{
            return;
		}

        OnTickSpeedUpdated?.Invoke(value);
    }

    public void ClearGrid()
    {
        OnClearGrid?.Invoke();
    }
}