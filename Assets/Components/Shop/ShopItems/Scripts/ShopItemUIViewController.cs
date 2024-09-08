using Components.Economie;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Components.Shop.ShopItems
{
	public class ShopItemUIViewController : MonoBehaviour
	{
		private ShopItem _shopItem;
		protected int _price;
		[SerializeField] protected Image _itemView;
		[SerializeField] protected TextMeshProUGUI _itemName;
		[SerializeField] protected TextMeshProUGUI _itemPrice;
		[SerializeField] protected GameObject _soldItemGO;

		public static Action<int> OnItemBuyed;
		public virtual void Init(ShopItem shopItem)
		{
			_shopItem = shopItem;
		}

		public void BuyItem()
		{
			if(EconomieController.PlayerMoney >=  _price)
			{
				OnItemBuyed?.Invoke(_price);
				_soldItemGO.SetActive(true);
			}
		}
	}
}
