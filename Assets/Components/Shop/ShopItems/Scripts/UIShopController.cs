using Components.Machines;
using Components.Shop.ShopItems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Shop
{
	public class UIShopController : MonoBehaviour
	{

		[Header("Shop")]
		[SerializeField] private GameObject _shopUIView;
		[SerializeField] private UIMachineShopItemViewController _machineShopUIViewController;
		[SerializeField] private UIBuyedItemController _buyedItemController;

		[SerializeField] private Transform _machineShopUIParent;
		[SerializeField] private Transform _itemsBuyedParent;

		private void Awake()
		{
			ShopController.OnShopGenerated += SetInfos;
			UIMachineShopItemViewController.OnMachineBuyed += AddMachineToBuyedItems;
		}


		private void OnDestroy()
		{
			ShopController.OnShopGenerated -= SetInfos;
		}

		public void SetInfos(List<ShopItem> shopItems)
		{

			//Destroying all child before instantiate new ones
			foreach (Transform child in _machineShopUIParent)
			{
				Destroy(child.gameObject);
			}


			//Instantiate all shop items
			foreach (ShopItem shopItem in shopItems)
			{
				//Check if the shopItem contains a machine
				if (shopItem.MachineTemplate != null)
				{
					UIMachineShopItemViewController UIMachineShopItemViewController = Instantiate(_machineShopUIViewController, _machineShopUIParent);
					UIMachineShopItemViewController.Init(shopItem);
				}
			}

		}

		public void OnOpenShop()
		{
			//Destroying all child before instantiate new ones
			foreach (Transform child in _itemsBuyedParent)
			{
				Destroy(child.gameObject);
			}
		}

		private void AddMachineToBuyedItems(MachineTemplate template)
		{
			var newItem = Instantiate(_buyedItemController, _itemsBuyedParent);
			newItem.SetInfos(template);
		}
	}

}
