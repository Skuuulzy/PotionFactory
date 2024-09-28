using Components.Economy;
using Components.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Shop.ShopItems
{
    public class UIConsumableShopItemViewController : UIShopItemViewController
	{
		public override void Init(ShopItem shopItem)
		{
			base.Init(shopItem);

			//Set parameters
			_itemName.text = shopItem.ConsumableTemplate.ConsumableName;
			_itemPrice.text = $"{shopItem.ConsumableTemplate.ShopPrice}";
			_itemView.sprite = shopItem.ConsumableTemplate.UIView;
			Price = shopItem.ConsumableTemplate.ShopPrice;
			_numberOfItemToSellText.text = shopItem.NumberOfItemToSell == -1 ? "\u221E" : $"{shopItem.NumberOfItemToSell}";
			CheckBuyingEligibility(EconomyController.Instance.PlayerMoney);
			EconomyController.OnPlayerMoneyUpdated += CheckBuyingEligibility;
		}

		private void OnDestroy()
		{
			EconomyController.OnPlayerMoneyUpdated -= CheckBuyingEligibility;

		}
		private void CheckBuyingEligibility(int playerMoney)
		{
			if (Price > playerMoney)
			{
				_itemPrice.color = Color.red;
			}
			else
			{
				_itemPrice.color = Color.white;
			}
		}

		public override void BuyItem()
		{
			if (EconomyController.Instance.PlayerMoney < Price)
				return;

			EconomyController.Instance.DecreaseMoney(Price);
			if (_shopItem.NumberOfItemToSell != -1)
			{
				if (_shopItem.NumberOfItemToSell == 0)
				{
					Debug.LogError("Should not be able to buy this item");
				}

				_shopItem.DecreaseNumberOfItemToSell();

				if (_shopItem.NumberOfItemToSell == 0)
				{
					_soldItemGO.SetActive(true);
				}
			}

			InventoryController.Instance.AddConsumableToPlayerInventory(_shopItem.ConsumableTemplate, 1);

		}
	}
}
