using Components.Grid;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GrimoireInventoryButton : MonoBehaviour, IPointerClickHandler
{
    [Header("Info button")]
    [SerializeField] private Image _buttonImage;
    [SerializeField] private Image _buttonBackground;
    [SerializeField] private Material[] _typeColor;
    [SerializeField] private Image _buttonForm;
    [SerializeField] private Sprite[] _typeForm;
    [SerializeField] private TextMeshProUGUI _numberOfAvailableText;

    [Header("Zero state")]
    [SerializeField] private Image _border;
    [SerializeField] private Sprite _normalBackground;
    [SerializeField] private Sprite _zeroBackground;
    private bool _zeroState = false;

    [Header("Button comportment")]
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

        _buttonImage.sprite = machine.UIView;
        _buttonBackground.material = _typeColor[(int)_type];
        _buttonForm.sprite = _typeForm[(int)_type];

        UpdateNumberOfAvailableMachine(value);
    }

    public void UpdateNumberOfAvailableMachine(int number)
    {
        _numberOfAvailableText.text = number.ToString();

        if (number <= 0)
        {
            _zeroState = true;
            _buttonBackground.sprite = _zeroBackground;

            Color alpha75 = _border.color;
            alpha75.a = 0.75f;
            _border.color = _buttonForm.color = _buttonImage.color = alpha75;

            OnDeselect();
        }
        else if (_zeroState == true)
        {
            _zeroState = false;
            _buttonBackground.sprite = _normalBackground;

            Color alpha100 = _border.color;
            alpha100.a = 1f;
            _border.color = _buttonForm.color = _buttonImage.color = alpha100;
        }
    }
    #endregion

    //-------------- BUTTON COMPORTMENT --------------//
    #region Button comportment
    public void OnHover()
    {
        if (_zeroState == true)
            return;

        if (_isSelected == false)
        {
            _animator.SetTrigger("Hover");
        }
        _animator.SetBool("isHover", true);
    }

    public void OnUnhover()
    {
        if (_zeroState == true)
            return;

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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnSelect();
        }
    }

    public void OnSelect()
    {
        if (_zeroState == true)
            return;

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
