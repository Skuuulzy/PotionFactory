using Components.Tick;
using SoWorkflow.SharedValues;
using TMPro;
using UnityEngine;
using VComponent.Tools.SceneLoader;

public class UIStateController : MonoBehaviour
{
	[SerializeField] private Animator _stateUITitleAnimator;
	[SerializeField] private TextMeshProUGUI _stateNameText;
	[SerializeField] private TextMeshProUGUI _stateCountdownText;

	[SerializeField] private SOSharedFloat _stateCountdownTime;

	[Header("EndGame")]
	[SerializeField] private GameObject _endGameGO;


	private static readonly int DISPLAY_STATE = Animator.StringToHash("DisplayState");
	private BaseState _currentState;

	private void Awake()
	{
        _stateCountdownTime.OnValueUpdated += SetCountdownTime;
		StateController.OnStateStarted += HandleStateStarted;
	}

	private void OnDestroy()
	{
        _stateCountdownTime.OnValueUpdated -= SetCountdownTime;
        StateController.OnStateStarted -= HandleStateStarted;
	}
	
	private void HandleStateStarted(BaseState state)
	{
		_currentState = state;

		switch (state)
		{
			case EndOfDayState endOfDayState:
				HideCountdown();

				break;
			case PlanningFactoryState planningFactoryState:
				break;
			case ResolutionFactoryState resolutionFactoryState:
				DisplayNewState(resolutionFactoryState);
				break;
			case EndGameState endGameState:
				HideCountdown();
				DisplayEndGameState();
				break;
			case GameOverState gameOverState:
				HideCountdown();
				break;
		}
	}


	private void DisplayNewState(BaseState state)
	{
		_stateNameText.text = $"Day {state.StateIndex}";
		_stateUITitleAnimator.SetTrigger(DISPLAY_STATE);
	}

	private void SetCountdownTime(float currentTime)
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