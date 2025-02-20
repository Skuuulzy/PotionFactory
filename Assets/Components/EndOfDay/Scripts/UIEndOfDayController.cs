using Components.Economy;
using Components.Order;
using Components.Shop.ShopItems;
using SoWorkflow.SharedValues;
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

		[Header("SharedValues")]
		[SerializeField] private SOSharedInt _stateScoreObjective;
		[SerializeField] private SOSharedInt _playerScore;
		[SerializeField] private SOSharedInt _totalGuildEarned;


		private void Start()
		{
			EndOfDayState.OnEndOfDayStateStarted += DisplayEndOfDay;
			EndOfDayState.OnEndOfDayStateEnded += HideEndOfDay;
		}

		private void OnDestroy()
		{
			EndOfDayState.OnEndOfDayStateStarted -= DisplayEndOfDay;
			EndOfDayState.OnEndOfDayStateEnded -= HideEndOfDay;
		}

		private void DisplayEndOfDay(EndOfDayState state)
		{
			_letterGO.SetActive(true);
			_orderDialogueController.gameObject.SetActive(true);
			_scrollView.SetActive(false);
			_endOfDayUIView.SetActive(true);

            _objectiveText.text = $"Your objective was {_stateScoreObjective.Value} gold coins, and you have reached {_playerScore.Value} gold coins.";
            _payoffText.text = $"As a result of your efforts, you have been awarded a total of {_totalGuildEarned.Value} units of the Order’s currency. Use it wisely, for the prosperity of the Order must always prevail.";
        }

		private void HideEndOfDay(EndOfDayState state)
		{
			_endOfDayUIView.SetActive(false);
		}
	}
}
