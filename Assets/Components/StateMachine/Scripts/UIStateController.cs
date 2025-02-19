using Components.Economy;
using Components.Tick;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VComponent.Tools.SceneLoader;

public class UIStateController : MonoBehaviour
{
	[SerializeField] private Animator _stateUITitleAnimator;
	[SerializeField] private TextMeshProUGUI _stateNameText;
	[SerializeField] private TextMeshProUGUI _stateCountdownText;
	[SerializeField] private Button _finishStateButton;

	[Header("EndGame")]
	[SerializeField] private GameObject _endGameGO;

	private static readonly int DISPLAY_STATE = Animator.StringToHash("DisplayState");
	private BaseState _currentState;

	private void Awake()
	{
		StateController.OnCountdown += SetCountdownTime;
		StateController.OnStateStarted += HandleStateStarted;
	}

	private void OnDestroy()
	{
		StateController.OnCountdown -= SetCountdownTime;
		StateController.OnStateStarted -= HandleStateStarted;
		
		EconomyController.OnStatePlayerScoreUpdated -= HandleScoreUpdated;
	}
	
	private void HandleStateStarted(BaseState state)
	{
		_currentState = state;

		switch (state)
		{
			case EndOfDayState endOfDayState:
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
				HideCountdown();
				DisplayEndGameState();
				_finishStateButton.gameObject.SetActive(false);
				break;
			case GameOverState gameOverState:
				HideCountdown();
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
		int currentTimeInSeconds = TickSystem.GetSecondValueFromTicks((int)currentTime);
        int seconds = (currentTimeInSeconds % 60);
        int minutes = (currentTimeInSeconds / 60);
        _stateCountdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
	}

	private void HideCountdown()
	{
		_stateCountdownText.text = $"--";
	}

	private void DisplayFinishStateButton(BaseState state)
	{
		_finishStateButton.gameObject.SetActive(true);
		_finishStateButton.onClick.AddListener(state.SetStateFinished);
		//_finishStateButtonText.text = 
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
		SceneLoader.LoadMainMenu();
	}
}