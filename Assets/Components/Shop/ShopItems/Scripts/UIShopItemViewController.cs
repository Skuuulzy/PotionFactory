using Components.Economy;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Components.Shop.ShopItems
{
	public class UIShopItemViewController : MonoBehaviour
	{
		[SerializeField] protected Image _itemView;
		[SerializeField] protected TextMeshProUGUI _itemName;
		[SerializeField] protected TextMeshProUGUI _itemPrice;
		[SerializeField] protected GameObject _soldItemGO;
		[SerializeField] protected TextMeshProUGUI _numberOfItemToSellText;

		protected int Price;
		protected ShopItem _shopItem;
		
		public virtual void Init(ShopItem shopItem)
		{
			_shopItem = shopItem;
		}

		public virtual void BuyItem()
		{
			
		}
	}
}