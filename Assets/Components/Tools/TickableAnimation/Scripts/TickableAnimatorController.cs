using Components.Tick;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickableAnimatorController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    private float _tickMultiplier = 1f;

    public void Start()
    {
        TickSystem.OnTickMultiplierChanged += HandleTickMultiplier;
        ApplyTickMultiplier();
    }

    private void OnDestroy()
    {
        TickSystem.OnTickMultiplierChanged -= HandleTickMultiplier;

    }

    private void ApplyTickMultiplier()
    {
        if (_animator != null)
        {
            _animator.speed = _tickMultiplier;
        }
    }
    private void HandleTickMultiplier(int multiplier)
    {
        _tickMultiplier = multiplier;
        ApplyTickMultiplier();
    }
}
