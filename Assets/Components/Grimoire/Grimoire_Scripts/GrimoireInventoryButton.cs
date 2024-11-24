using Components.Grid;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class GrimoireInventoryButton : MonoBehaviour
{
    [Header ("Info button")]
    [SerializeField] private Image _ButotnImage;
    [SerializeField] private Image _buttonBackground;
    [SerializeField] private Material[] _typeColor;
    [SerializeField] private Image _ButtonForm;
    [SerializeField] private Sprite[] _typeForm;
    [SerializeField] private TextMeshProUGUI _numberOfAvailableText;

    [Header ("Button comportment")]
    [SerializeField] private Animator _animator;

    private ShopItemType _type;

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

    //-------------- INFO BUTTON --------------//
    #region Info button

    public void InitMachine(Components.Machines.MachineTemplate machine, int value = 1)
    {
        _type = ShopItemType.MACHINE;

        _ButotnImage.sprite = machine.UIView;
        _buttonBackground.material = _typeColor[(int)_type];
        _ButtonForm.sprite = _typeForm[(int)_type];

        UpdateNumberOfAvailableMachine(value);
    }

    public void UpdateNumberOfAvailableMachine(int number)
    {
        _numberOfAvailableText.text = number.ToString();
    }
    #endregion

    //-------------- BUTTON COMPORTMENT --------------//
    #region Button comportment
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
    #endregion
}
