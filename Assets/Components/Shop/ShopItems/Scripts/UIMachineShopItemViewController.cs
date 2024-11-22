using Components.Economy;
using Components.Inventory;
using System;
using UnityEngine;

namespace Components.Shop.ShopItems
{
    public class UIMachineShopItemViewController : UIShopItemViewController
    {
        public override void Init(ShopItem shopItem)
        {
            base.Init(shopItem);

			//Set parameters
            _itemName.text = shopItem.MachineTemplate.Name;
            _itemPrice.text = $"{shopItem.MachineTemplate.ShopPrice}";
            _itemView.sprite = shopItem.MachineTemplate.UIView;
            Price = shopItem.MachineTemplate.ShopPrice;
			_numberOfItemToSellText.text = shopItem.NumberOfItemToSell == -1 ? "\u221E" :  $"{shopItem.NumberOfItemToSell}";

			CheckBuyingEligibility(EconomyController.Instance.PlayerMoney);
			EconomyController.OnPlayerMoneyUpdated += CheckBuyingEligibility;
		}

		private void OnDestroy()
		{
			EconomyController.OnPlayerMoneyUpdated -= CheckBuyingEligibility;

		}
		private void CheckBuyingEligibility(int playerMoney)
		{
			if(Price > playerMoney)
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
			{
				Debug.LogError("Not enough money you bum.");
				return;
			}

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

			GrimoireController.Instance.AddMachineToPlayerInventory(_shopItem.MachineTemplate, 1);

		}
	}
}
