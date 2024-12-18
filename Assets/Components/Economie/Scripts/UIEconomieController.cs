using System.Collections;
using TMPro;
using UnityEngine;

namespace Components.Economy
{
    public class UIEconomieController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _playerMoneyText;
        [SerializeField] private TextMeshProUGUI _playerStateScoreText;
        [SerializeField] private TextMeshProUGUI _scoreStateObjectiveText;
        [SerializeField] private float _animDuration = 0.5f;

        private int _currentPlayerMoney = 0;
        private int _currentPlayerScore = 0;
        private void Start()
        {
	        _playerMoneyText.text = "0";
            EconomyController.OnPlayerMoneyUpdated += UpdateUIPlayerMoney;
            EconomyController.OnStatePlayerScoreUpdated += UpdateUIStatePlayerScore;
            EconomyController.OnScoreStateObjectiveUpdated += UpdateScoreStateObjective;
		}

		private void OnDestroy()
		{
            EconomyController.OnPlayerMoneyUpdated -= UpdateUIPlayerMoney;
            EconomyController.OnStatePlayerScoreUpdated -= UpdateUIStatePlayerScore;
            EconomyController.OnScoreStateObjectiveUpdated -= UpdateScoreStateObjective;
		}

		private void UpdateUIPlayerMoney(int playerMoney)
		{
            StopAllCoroutines();
            StartCoroutine(AnimateValueCoroutine(_playerMoneyText,_currentPlayerMoney, playerMoney, _animDuration));
            _currentPlayerMoney = playerMoney;
		}

		private void UpdateUIStatePlayerScore(int playerStateMoney)
		{
            StopAllCoroutines();
            StartCoroutine(AnimateValueCoroutine(_playerStateScoreText, _currentPlayerScore, playerStateMoney, _animDuration));
            _currentPlayerScore = playerStateMoney;
		}

		private void UpdateScoreStateObjective(int money)
		{
			_scoreStateObjectiveText.text = $"Objective {money}";
		}

        private IEnumerator AnimateValueCoroutine(TextMeshProUGUI text, int startValue, int endValue, float duration)
        {
            int currentValue = startValue;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / duration);
                currentValue = Mathf.RoundToInt(Mathf.Lerp(startValue, endValue, progress));
                UpdateText(text, currentValue);
                yield return null;
            }

            currentValue = endValue;
            UpdateText(text, currentValue);
        }

        private void UpdateText(TextMeshProUGUI text, int currentValue)
        {
            text.text = $"{currentValue}";
        }

        public void DebugAddScore()
		{
			EconomyController.Instance.AddScore(1000);
		}
		public void DebugAddMoney()
		{
			EconomyController.Instance.AddMoney(10);
		}
	}
}