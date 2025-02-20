using SoWorkflow.SharedValues;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Components.Economy
{
    public class UIEconomieController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _playerGuildTokenText;
        [SerializeField] private TextMeshProUGUI _playerStateScoreText;
        [SerializeField] private TextMeshProUGUI _scoreStateObjectiveText;
        [SerializeField] private TextMeshProUGUI _interestTokenText;
        [SerializeField] private Button _finishStateButton;

        [Header("SharedValues")]
        [SerializeField] private SOSharedInt _playerGuildToken;
        [SerializeField] private SOSharedInt _playerScore;
        [SerializeField] private SOSharedInt _stateScoreObjective;
        [SerializeField] private SOSharedInt _guildTokenTimeInterest;
            
        [SerializeField] private float _animDuration = 0.5f;

        private int _currentPlayerMoney = 0;
        private int _currentPlayerScore = 0;
        private Coroutine _moneyCoroutine;
        private Coroutine _scoreCoroutine;

        private void Start()
        {
            _playerGuildTokenText.text = "0";
            _playerGuildToken.OnValueUpdated += UpdateUIPlayerGuildToken;
            _playerScore.OnValueUpdated += UpdateUIStatePlayerScore;
            _stateScoreObjective.OnValueUpdated += UpdateScoreStateObjective;
            _guildTokenTimeInterest.OnValueUpdated += UpdateGuildTokenInterest;
        }

        private void OnDestroy()
        {
            _playerGuildToken.OnValueUpdated -= UpdateUIPlayerGuildToken;
            _playerScore.OnValueUpdated -= UpdateUIStatePlayerScore;
            _stateScoreObjective.OnValueUpdated -= UpdateScoreStateObjective;
            _guildTokenTimeInterest.OnValueUpdated -= UpdateGuildTokenInterest;

        }



        /// <summary>
        /// Updates the player's money display with an animated transition.
        /// </summary>
        private void UpdateUIPlayerGuildToken(int playerMoney)
        {
            if (_moneyCoroutine != null)
                StopCoroutine(_moneyCoroutine);

            _moneyCoroutine = StartCoroutine(AnimateValueCoroutine(_playerGuildTokenText, _currentPlayerMoney, playerMoney, _animDuration));
            _currentPlayerMoney = playerMoney;
        }

        /// <summary>
        /// Updates the player's score in the state display with an animated transition.
        /// </summary>
        private void UpdateUIStatePlayerScore(int score)
        {
            if (_scoreCoroutine != null)
                StopCoroutine(_scoreCoroutine);

            _scoreCoroutine = StartCoroutine(AnimateValueCoroutine(_playerStateScoreText, _currentPlayerScore, score, _animDuration));
            _currentPlayerScore = score;

            _finishStateButton.gameObject.SetActive(score != 0 && score >= _stateScoreObjective.Value);
        }

        /// <summary>
        /// Updates the score objective display instantly.
        /// </summary>
        private void UpdateScoreStateObjective(int money)
        {
            _scoreStateObjectiveText.text = $"<b>Objectif</b> {money}";
        }

        private void UpdateGuildTokenInterest(int value)
        {
            _interestTokenText.text = $"+{value}";
        }
        /// <summary>
        /// Animates the transition of a numerical value over time.
        /// </summary>
        private IEnumerator AnimateValueCoroutine(TextMeshProUGUI text, int startValue, int endValue, float duration)
        {
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / duration);
                int currentValue = Mathf.RoundToInt(Mathf.Lerp(startValue, endValue, progress));
                text.text = currentValue.ToString();
                yield return null;
            }
            text.text = endValue.ToString();
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