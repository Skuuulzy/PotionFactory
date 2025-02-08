using System;
using Components.Economy;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VComponent.Tools.SceneLoader;

public class UIStateController : MonoBehaviour
{
	[SerializeField] private Animator _stateUITitleAnimator;
	[SerializeField] private TextMeshProUGUI _stateNameText;
	[SerializeField] private TextMeshProUGUI _stateCountdownText;
	[SerializeField] private Image _stateCountdownImage;
	[SerializeField] private Button _finishStateButton;

	[Header("EndGame")]
	[SerializeField] private GameObject _endGameGO;

	[Header("GameOver")]
	[SerializeField] private GameObject _gameOverGO;

	private static readonly int DISPLAY_STATE = Animator.StringToHash("DisplayState");
	private BaseState _currentState;

	private void Awake()
	{
		EconomyController.OnGameOver += HandleGameOver;
		StateController.OnCountdown += SetCountdownTime;
		StateController.OnStateStarted += HandleStateStarted;
	}

	private void OnDestroy()
	{
		EconomyController.OnGameOver -= HandleGameOver;
		StateController.OnCountdown -= SetCountdownTime;
		StateController.OnStateStarted -= HandleStateStarted;
		
		EconomyController.OnStatePlayerScoreUpdated -= HandleScoreUpdated;
	}
	
	private void HandleStateStarted(BaseState state)
	{
		_currentState = state;

		switch (state)
		{
			case EndOfDayState shopState:
				HideCountdown();
				_finishStateButton.gameObject.SetActive(false);
				break;
			case PlanningFactoryState planningFactoryState:
				break;
			case ResolutionFactoryState resolutionFactoryState:
				EconomyController.OnStatePlayerScoreUpdated -= HandleScoreUpdated;
				EconomyController.OnStatePlayerScoreUpdated += HandleScoreUpdated;
				DisplayNewState(resolutionFactoryState);
				_finishStateButton.gameObject.SetActive(false);
				break;
			case EndGameState endGameState:
				DisplayEndGameState();
				_finishStateButton.gameObject.SetActive(false);
				break;
		}
	}

	private void HandleScoreUpdated(int score)
	{
		if (score >= EconomyController.Instance.StateScoreObjective)
		{
			DisplayFinishStateButton(_currentState);
			EconomyController.OnStatePlayerScoreUpdated -= HandleScoreUpdated;
		}
	}

	private void DisplayNewState(BaseState state)
	{
		_stateNameText.text = $"Day {state.StateIndex}";
		_stateUITitleAnimator.SetTrigger(DISPLAY_STATE);
	}

	private void SetCountdownTime(float currentTime, float duration)
	{
		//Check if the timer is display 
		/*
		if (_stateCountdownImage.gameObject.activeSelf == false)
		{
			_stateCountdownImage.gameObject.SetActive(true);
		}

        _stateCountdownImage.fillAmount = currentTime / duration;
		*/

		_stateCountdownText.text = $"{ currentTime}";
	}

	private void HideCountdown()
	{
		_stateCountdownImage.gameObject.SetActive(false);
	}

	private void DisplayFinishStateButton(BaseState state)
	{
		_finishStateButton.gameObject.SetActive(true);
		_finishStateButton.onClick.AddListener(state.SetStateFinished);
	}

	private void HandleGameOver()
	{
		_gameOverGO.SetActive(true);
	}

	public void OnEndCurrentState()
	{
		_currentState.SetStateFinished();
	}

	public void DisplayEndGameState()
	{
		_endGameGO.SetActive(true);
	}

	// TODO: Is it supposed to be here ? 
	public void ReturnToMainMenu()
	{
		SceneLoader.Instance.LoadMainMenu();
	}
}