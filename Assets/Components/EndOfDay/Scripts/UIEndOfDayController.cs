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
			_objectiveText.text = $"Your objective was {objectiveScore} gold coins, and you have reached {playerScore} gold coins.";
			_payoffText.text = $"As a result of your efforts, you have been awarded a total of {totalGoldEarned} units of the Order’s currency. Use it wisely, for the prosperity of the Order must always prevail.";
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
