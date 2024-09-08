using TMPro;
using UnityEngine;

namespace Components.Economie
{
    public class UIEconomieController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _playerMoneyText;

        private void Start()
        {
	        _playerMoneyText.text = "0";
            EconomieController.OnPlayerMoneyUpdate += UpdateUIPlayerMoney;
		}

		private void OnDestroy()
		{
            EconomieController.OnPlayerMoneyUpdate -= UpdateUIPlayerMoney;
		}

		private void UpdateUIPlayerMoney(int playerMoney)
		{
            _playerMoneyText.text = $"{playerMoney}";
		}
    }
}