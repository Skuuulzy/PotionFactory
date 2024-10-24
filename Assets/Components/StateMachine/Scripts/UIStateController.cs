using Components.Economy;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStateController : MonoBehaviour
{
	[SerializeField] private Animator _stateUITitleAnimator;
	[SerializeField] private TextMeshProUGUI _stateNameText;
	[SerializeField] private Image _stateCountdownImage;
	[SerializeField] private Button _finishStateButton;

	[Header("GameOver")]
	[SerializeField] private GameObject _gameOverGO;

	private void Awake()
	{
		BaseState.OnStateStarted += DisplayNewState;
		EconomyController.OnGameOver += HandleGameOver;
	}

	private void OnDestroy()
	{
		BaseState.OnStateStarted -= DisplayNewState;
		EconomyController.OnGameOver -= HandleGameOver;
	}

	private void DisplayNewState(BaseState state)
	{
		_stateNameText.text = $"{state.StateName} : {state.StateIndex}";
		_stateUITitleAnimator.SetTrigger("DisplayState");
	}

	public void SetCountdownTime(float currentTime, float duration)
	{
		//Check if the timer is display 
		if (_stateCountdownImage.gameObject.activeSelf == false)
		{
			_stateCountdownImage.gameObject.SetActive(true);
		}

		_stateCountdownImage.fillAmount = currentTime / duration;
	}

	public void HideCountdown()
	{
		_stateCountdownImage.gameObject.SetActive(false);
	}

	public void DisplayFinishStateButton(BaseState state)
	{
		_finishStateButton.gameObject.SetActive(true);
		_finishStateButton.onClick.AddListener(state.SetStateFinished);
	}

	private void HandleGameOver()
	{
		_gameOverGO.SetActive(true);
	}
}