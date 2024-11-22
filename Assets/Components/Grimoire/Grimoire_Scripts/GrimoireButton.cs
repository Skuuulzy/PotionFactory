using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GrimoireButton : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    private bool _isSelected = false;

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

    public void OnSelect()
    {
        if (_isSelected == false)
        {
            _animator.SetBool("Selected", true);
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
