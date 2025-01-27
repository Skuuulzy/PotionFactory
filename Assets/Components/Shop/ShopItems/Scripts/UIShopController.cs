using Components.Economy;
using Components.Shop.ShopItems;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Components.Shop.UI
{
	public class UIShopController : MonoBehaviour
	{
		[SerializeField] private GameObject _shopUIView;

		[Header("PayOff")]
		[SerializeField] private TextMeshProUGUI _objectiveText;
		[SerializeField] private TextMeshProUGUI _resultText;
		[SerializeField] private TextMeshProUGUI _payoffText;

		[Header("Shop")]
		[SerializeField] private UIMachineShopItemViewController _machineShopUIViewController;
		[SerializeField] private UIConsumableShopItemViewController _consumableShopUIViewController;
		[SerializeField] private UIRelicShopItemViewController _relicShopUIViewController;
		[SerializeField] private Transform _machineShopUIParent;
		[SerializeField] private Transform _consumableShopUIParent;
		[SerializeField] private Transform _relicShopUIParent;

		private void Start()
		{
			ShopController.OnShopGenerated += DisplayShopItems;
			EconomyController.OnEndRoundGoldValuesCalculated += DisplayPayOffInfos;
			ShopState.OnShopStateEnded += HideShop;
		}

		private void OnDestroy()
		{
			ShopController.OnShopGenerated -= DisplayShopItems;
			EconomyController.OnEndRoundGoldValuesCalculated -= DisplayPayOffInfos;
			ShopState.OnShopStateEnded -= HideShop;
		}

		private void DisplayPayOffInfos(int totalGoldEarned, int baseGoldAmount, int goldInterest, int objectiveScore, int playerScore)
		{
			_objectiveText.text = $"Your objective was to make <b><color=red>{objectiveScore}</b></color> golds";
			_resultText.text = $"You succeeded by obtaining <b><color=red>{playerScore}</b></color> golds";
			_payoffText.text = $"The order rewards you by granting you <b><color=#EF33E9>{totalGoldEarned}</b></color> order tickets.\n \n Use them as you wish within our market";
		}

		private void DisplayShopItems(List<ShopItem> shopItems)
		{
			//Destroying all child before instantiate new ones
			foreach (Transform child in _machineShopUIParent)
			{
				Destroy(child.gameObject);
			}

			foreach (Transform child in _consumableShopUIParent)
			{
				Destroy(child.gameObject);
			}

			foreach (Transform child in _relicShopUIParent)
			{
				Destroy(child.gameObject);
			}

			_shopUIView.SetActive(true);

			//Instantiate all shop items
			foreach (ShopItem shopItem in shopItems)
			{
				//Check if the shopItem contains a machine
				if (shopItem.MachineTemplate != null)
				{
					UIMachineShopItemViewController UIMachineShopItemViewController = Instantiate(_machineShopUIViewController, _machineShopUIParent);
					UIMachineShopItemViewController.Init(shopItem);
				}
				else if(shopItem.ConsumableTemplate != null)
				{
					UIShopItemViewController UIShopItemViewController = Instantiate(_consumableShopUIViewController, _consumableShopUIParent);
					UIShopItemViewController.Init(shopItem);
				}
				else if(shopItem.RelicTemplate != null)
				{
					UIRelicShopItemViewController UIRelicShopItemViewController = Instantiate(_relicShopUIViewController, _relicShopUIParent);
					UIRelicShopItemViewController.Init(shopItem);
				}
			}
		}

		private void HideShop(ShopState state)
		{
			_shopUIView.SetActive(false);
		}
	}
}
