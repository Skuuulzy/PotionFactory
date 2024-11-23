using Components.Grid;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GrimoireInventoryButton : MonoBehaviour
{
    [SerializeField] private Image _background;
    [SerializeField] private TextMeshProUGUI _numberOfAvailableText;
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

    //When click on a selected button or another button, we deselect the selected
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
        }
    }

    public void OnDeselect()
    {
        _animator.SetBool("Selected", false);
        _isSelected = false;
    }
}
