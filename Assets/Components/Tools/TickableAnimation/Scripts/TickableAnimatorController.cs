using Components.Tick;
using SoWorkflow.SharedValues;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickableAnimatorController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private SOSharedInt _tickMultiplier;

    public void Start()
    {
        ApplyTickMultiplier(_tickMultiplier.Value);
        _tickMultiplier.OnValueUpdated += ApplyTickMultiplier;
    }
    private void OnDestroy()
    {
        _tickMultiplier.OnValueUpdated -= ApplyTickMultiplier;
    }

    private void ApplyTickMultiplier(int value)
    {
        if (_animator != null)
        {
            _animator.speed = value;
        }
    }
}
