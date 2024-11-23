using Components.Economy;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIStateController : MonoBehaviour
{
	[SerializeField] private Animator _stateUITitleAnimator;
	[SerializeField] private TextMeshProUGUI _stateNameText;
	[SerializeField] private Image _stateCountdownImage;
	[SerializeField] private Button _finishStateButton;

	[Header("GameOver")]
	[SerializeField] private GameObject _gameOverGO;

	private static readonly int DISPLAY_STATE = Animator.StringToHash("DisplayState");

	private void Awake()
	{
		BaseState.OnStateStarted += DisplayNewState;
		EconomyController.OnGameOver += HandleGameOver;
		StateController.OnCountdown += SetCountdownTime;
		StateController.OnStateStarted += HandleStateStarted;
	}

	private void OnDestroy()
	{
		BaseState.OnStateStarted -= DisplayNewState;
		EconomyController.OnGameOver -= HandleGameOver;
		StateController.OnCountdown -= SetCountdownTime;
		StateController.OnStateStarted -= HandleStateStarted;
	}
	
	private void HandleStateStarted(BaseState state)
	{
		switch (state)
		{
			case ShopState shopState:
				HideCountdown();
				DisplayFinishStateButton(shopState);
				break;
			case PlanningFactoryState planningFactoryState:
				DisplayFinishStateButton(planningFactoryState);
				break;
			case ResolutionFactoryState resolutionFactoryState:
				DisplayFinishStateButton(resolutionFactoryState);
				break;
		}
	}

	private void DisplayNewState(BaseState state)
	{
		_stateNameText.text = $"{state.StateName} : {state.StateIndex}";
		_stateUITitleAnimator.SetTrigger(DISPLAY_STATE);
	}

	private void SetCountdownTime(float currentTime, float duration)
	{
		//Check if the timer is display 
		if (_stateCountdownImage.gameObject.activeSelf == false)
		{
			_stateCountdownImage.gameObject.SetActive(true);
		}

		_stateCountdownImage.fillAmount = currentTime / duration;
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

	// TODO: MOVE ELSEWHERE
	public void ReturnToMainMenu()
	{
		SceneManager.LoadScene("Main_Menu", LoadSceneMode.Single);
	}
}