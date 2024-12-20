using System;
using UnityEngine;

public class UIOptionsController : MonoBehaviour
{
    public static Action<int> OnTickSpeedUpdated;
    public static Action OnClearGrid;
    
    public void ChangeTimeSpeed(int value)
    {
        OnTickSpeedUpdated?.Invoke(value);
    }

    public void ClearGrid()
    {
        OnClearGrid?.Invoke();
    }
}