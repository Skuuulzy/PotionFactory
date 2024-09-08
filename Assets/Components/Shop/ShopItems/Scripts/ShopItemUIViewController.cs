using Components.Economy;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Components.Shop.ShopItems
{
	public class ShopItemUIViewController : MonoBehaviour
	{
		[SerializeField] protected Image _itemView;
		[SerializeField] protected TextMeshProUGUI _itemName;
		[SerializeField] protected TextMeshProUGUI _itemPrice;
		[SerializeField] protected GameObject _soldItemGO;

		protected int Price;
		private ShopItem _shopItem;
		
		public virtual void Init(ShopItem shopItem)
		{
			_shopItem = shopItem;
		}

		public void BuyItem()
		{
			if (EconomyController.Instance.PlayerMoney < Price) 
				return;
			
			EconomyController.Instance.RemoveMoney(Price);
			
			_soldItemGO.SetActive(true);
		}
	}
}