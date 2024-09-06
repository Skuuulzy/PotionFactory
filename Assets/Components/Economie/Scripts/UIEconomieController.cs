using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Components.Economie
{
    public class UIEconomieController : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI _playerMoneyText;

        // Start is called before the first frame update
        void Start()
        {
            EconomieController.OnPlayerMoneyUpdate += UpdateUIPlayerMoney;
		}

		private void OnDestroy()
		{
            EconomieController.OnPlayerMoneyUpdate -= UpdateUIPlayerMoney;
		}

		private void UpdateUIPlayerMoney(int playerMoney)
		{
            _playerMoneyText.text = $"Player Money : {playerMoney}";
		}

    }
}
