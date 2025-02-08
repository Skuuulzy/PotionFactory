using Components.Economy;
using Components.Order;
using Components.Shop.ShopItems;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Components.Shop.UI
{
	public class UIEndOfDayController : MonoBehaviour
	{
		[SerializeField] private GameObject _endOfDayUIView;
		[SerializeField] private GameObject _letterGO;
		[SerializeField] private GameObject _scrollView;
		[SerializeField] private OrderDialogueController _orderDialogueController;

		[Header("PayOff")]
		[SerializeField] private TextMeshProUGUI _objectiveText;
		[SerializeField] private TextMeshProUGUI _resultText;
		[SerializeField] private TextMeshProUGUI _payoffText;


		private void Start()
		{
			EndOfDayState.OnEndOfDayStateStarted += DisplayEndOfDay;
			EconomyController.OnEndRoundGoldValuesCalculated += DisplayPayOffInfos;
			EndOfDayState.OnEndOfDayStateEnded += HideEndOfDay;
		}

		private void OnDestroy()
		{
			EndOfDayState.OnEndOfDayStateStarted -= DisplayEndOfDay;
			EconomyController.OnEndRoundGoldValuesCalculated -= DisplayPayOffInfos;
			EndOfDayState.OnEndOfDayStateEnded -= HideEndOfDay;
		}

		private void DisplayPayOffInfos(int totalGoldEarned, int baseGoldAmount, int goldInterest, int objectiveScore, int playerScore)
		{
			_objectiveText.text = $"Your objective was to make <b><color=red>{objectiveScore}</b></color> golds";
			_resultText.text = $"You succeeded by obtaining <b><color=red>{playerScore}</b></color> golds";
			_payoffText.text = $"The order rewards you by granting you <b><color=#EF33E9>{totalGoldEarned}</b></color> order tickets.\n \n Use them as you wish within our market";
		}

		private void DisplayEndOfDay(EndOfDayState state)
		{
			_letterGO.SetActive(true);
			_orderDialogueController.gameObject.SetActive(true);
			_scrollView.SetActive(false);

			_endOfDayUIView.SetActive(true);
		}

		private void HideEndOfDay(EndOfDayState state)
		{
			_endOfDayUIView.SetActive(false);
		}
	}
}
