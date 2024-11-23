using Components.Grid;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrimoireInventoryButton : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    private bool _isSelected = false;

    public static Action OnSelected;

    private void Start()
    {
        OnSelected += HandleOnSelected;
        GridPreviewController.OnPreviewUnselected += HandleOnDeselected;
    }

    private void OnDestroy()
    {
        OnSelected -= HandleOnSelected;
        GridPreviewController.OnPreviewUnselected -= HandleOnDeselected;
    }
    public void OnHover()
    {
        _animator.SetTrigger("Hover");
    }

    public void OnUnhover()
    {
        if (_isSelected == false)
        {
            _animator.SetTrigger("Unhover");
        }
    }

    private void HandleOnSelected()
    {
        if (_isSelected == true)
        {
            OnDeselect();
        }
    }
    private void HandleOnDeselected(bool asPreview)
    {
        if (_isSelected == true && asPreview == true)
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
            OnHover();
        }
    }

    public void OnDeselect()
    {
        _animator.SetBool("Selected", false);
        _isSelected = false;
    }
}
