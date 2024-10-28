using TMPro;
using UnityEngine;

namespace Components.Economy
{
    public class UIEconomieController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _playerMoneyText;
        [SerializeField] private TextMeshProUGUI _playerStateMoneyText;
        [SerializeField] private TextMeshProUGUI _moneyStateObjectiveText;

        private void Start()
        {
	        _playerMoneyText.text = "0";
            EconomyController.OnPlayerMoneyUpdated += UpdateUIPlayerMoney;
            EconomyController.OnStatePlayerMoneyUpdated += UpdateUIStatePlayerMoney;
            EconomyController.OnMoneyStateObjectiveUpdated += UpdateMoneyStateObjective;
		}

		private void OnDestroy()
		{
            EconomyController.OnPlayerMoneyUpdated -= UpdateUIPlayerMoney;
            EconomyController.OnStatePlayerMoneyUpdated -= UpdateUIStatePlayerMoney;
            EconomyController.OnMoneyStateObjectiveUpdated -= UpdateMoneyStateObjective;
		}

		private void UpdateUIPlayerMoney(int playerMoney)
		{
            _playerMoneyText.text = $"{playerMoney}";
		}

		private void UpdateUIStatePlayerMoney(int playerStateMoney)
		{
			_playerStateMoneyText.text = $"{playerStateMoney}";
		}

		private void UpdateMoneyStateObjective(int money)
		{
			_moneyStateObjectiveText.text = $"Objective {money}";
		}

		public void DebugAddMoney()
		{
			EconomyController.Instance.AddMoney(1000);
		}
    }
}