using TMPro;
using UnityEngine;

namespace Components.Economy
{
    public class UIEconomieController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _playerMoneyText;
        [SerializeField] private TextMeshProUGUI _playerStateScoreText;
        [SerializeField] private TextMeshProUGUI _scoreStateObjectiveText;

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
            _playerMoneyText.text = $"{playerMoney}";
		}

		private void UpdateUIStatePlayerScore(int playerStateMoney)
		{
			_playerStateScoreText.text = $"{playerStateMoney}";
		}

		private void UpdateScoreStateObjective(int money)
		{
			_scoreStateObjectiveText.text = $"Objective {money}";
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