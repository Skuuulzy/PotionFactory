using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIStateController : MonoBehaviour
{
	[SerializeField] private Animator _stateUITitleAnimator;
	[SerializeField] private TextMeshProUGUI _stateNameText;

	private void Start()
	{
		BaseState.OnStateStarted += DisplayNewState;
	}

	private void DisplayNewState(string stateName)
	{
		_stateNameText.text = stateName;
		_stateUITitleAnimator.SetTrigger("DisplayState");
	}
}
