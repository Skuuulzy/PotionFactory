using Components.Grid;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class GrimoireButton : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    private bool _isSelected = false;

    private static Action OnSelected;

    private void Start()
    {
        OnSelected += HandleOnSelect;
        GridPreviewController.OnPreviewUnselected += HandleOnDeselected;
    }

    private void OnDestroy()
    {
        OnSelected -= HandleOnSelect;
        GridPreviewController.OnPreviewUnselected -= HandleOnDeselected;
    }

    public void OnHover()
    {
        if (_isSelected == false)
        {
            _animator.SetTrigger("Hover");
        }
        _animator.SetBool("isHover", true);
    }

    public void OnUnhover()
    {
        if (_isSelected == false)
        {
            _animator.SetTrigger("Unhover");
        }
        _animator.SetBool("isHover", false);
    }

    private void HandleOnSelect()
    {
        if (_isSelected == true)
        {
            OnDeselect();
        }
    }

    private void HandleOnDeselected(bool asPreview)
    {
        if (_isSelected == true && asPreview == false)
        {
            OnDeselect();
        }
    }

    public void OnSelect()
    {
        if (_isSelected == false)
        {
            _animator.SetBool("Selected", true);
            OnSelected?.Invoke();
            _isSelected = true;
        }
        else
        {
            OnDeselect();
        }
    }

    public void OnDeselect()
    {
        _animator.SetBool("Selected", false);
        _isSelected = false;
    }
}
