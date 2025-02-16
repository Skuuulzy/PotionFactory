using Components.Grid;
using Components.Inventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class GrimoireButton : MonoBehaviour
{
    [SerializeField] private ShopItemType _type;
    [SerializeField] private UIGrimoireController _uiGrimoireController;
    [SerializeField] private Animator _animator;
    private bool _isSelected = false;

    private static Action OnSelected;
    public static Action OnGrimoireButtonDeselect;

    private void Start()
    {
        OnSelected += HandleOnSelected;
        GridObjectInstantiator.OnPreview += HandleOnDeselected;
    }

    private void OnDestroy()
    {
        OnSelected -= HandleOnSelected;
        GridObjectInstantiator.OnPreview -= HandleOnDeselected;
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

    //When click on another button, we deselect the selected and close his inventory
    private void HandleOnSelected()
    {
        if (_isSelected == true)
        {
            OnDeselect();
        }
    }

    // When player right click with a machine preview
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
            _uiGrimoireController.ToggleInventory((int)_type);
            _animator.SetBool("Selected", true);
            OnSelected?.Invoke();
            _isSelected = true;
        }
        else
        {
            OnDeselect();
        }

        OnGrimoireButtonDeselect?.Invoke(); //Remove 3D preview when select or deselect another main button.
    }

    public void OnDeselect()
    {
        _uiGrimoireController.ToggleInventory((int)_type);
        _animator.SetBool("Selected", false);
        _isSelected = false;
    }
}
