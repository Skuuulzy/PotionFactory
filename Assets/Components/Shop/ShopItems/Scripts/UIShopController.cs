using Components.Shop.ShopItems;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Shop.UI
{
	public class UIShopController : MonoBehaviour
	{
		[SerializeField] private GameObject _shopUIView;
		[SerializeField] private UIMachineShopItemViewController _machineShopUIViewController;
		[SerializeField] private UIConsumableShopItemViewController _consumableShopUIViewController;
		[SerializeField] private UIRelicShopItemViewController _relicShopUIViewController;
		[SerializeField] private Transform _machineShopUIParent;
		[SerializeField] private Transform _consumableShopUIParent;
		[SerializeField] private Transform _relicShopUIParent;

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
