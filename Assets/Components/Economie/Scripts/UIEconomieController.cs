using TMPro;
using UnityEngine;

namespace Components.Economy
{
    public class UIEconomieController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _playerMoneyText;

        private void Start()
        {
	        _playerMoneyText.text = "0";
            EconomyController.OnPlayerMoneyUpdated += UpdateUIPlayerMoney;
		}

		private void OnDestroy()
		{
            EconomyController.OnPlayerMoneyUpdated -= UpdateUIPlayerMoney;
		}

		private void UpdateUIPlayerMoney(int playerMoney)
		{
            _playerMoneyText.text = $"{playerMoney}";
		}

		public void DebugAddMoney()
		{
			EconomyController.Instance.AddMoney(1000);
		}
    }
}