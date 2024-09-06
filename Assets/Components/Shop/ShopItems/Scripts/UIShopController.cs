using Components.Shop.ShopItems;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Shop.UI
{
	public class UIShopController : MonoBehaviour
	{
		[SerializeField] private GameObject _shopUIView;
		[SerializeField] private MachineShopItemUIViewController _machineShopUIViewController;
		[SerializeField] private Transform _machineShopUIParent;

		void Start()
		{
			ShopController.OnShopGenerated += DisplayShopItems;
			ShopState.OnShopStateEnded += HideShop;
		}

		private void OnDestroy()
		{
			ShopController.OnShopGenerated -= DisplayShopItems;
			ShopState.OnShopStateEnded -= HideShop;

		}

		private void DisplayShopItems(List<ShopItem> shopItems)
		{
			//Destroying all child before instantiate new ones
			foreach (Transform child in _machineShopUIParent)
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
					MachineShopItemUIViewController machineShopUIViewController = Instantiate(_machineShopUIViewController, _machineShopUIParent);
					machineShopUIViewController.Init(shopItem);
				}
			}
		}

		private void HideShop(ShopState state)
		{
			_shopUIView.SetActive(false);
		}


	}
}
