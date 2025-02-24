using Components.Economy;
using Components.Inventory;
using Components.Machines;
using SoWorkflow.SharedValues;
using System;
using UnityEngine;

namespace Components.Shop.ShopItems
{
    public class UIMachineShopItemViewController : UIShopItemViewController
    {
		[SerializeField] private SOSharedInt _playerGuildToken;
		public static Action<MachineTemplate> OnMachineBuyed;
        public override void Init(ShopItem shopItem)
        {
            base.Init(shopItem);

			//Set parameters
            _itemName.text = shopItem.MachineTemplate.Name;
            _itemPrice.text = $"{shopItem.MachineTemplate.ShopPrice}";
            _itemView.sprite = shopItem.MachineTemplate.UIView;
            Price = shopItem.MachineTemplate.ShopPrice;
			_numberOfItemToSellText.text = shopItem.NumberOfItemToSell == -1 ? "\u221E" :  $"{shopItem.NumberOfItemToSell}";

			CheckBuyingEligibility(_playerGuildToken.Value);
            _playerGuildToken.OnValueUpdated += CheckBuyingEligibility;
		}

		private void OnDestroy()
		{
            _playerGuildToken.OnValueUpdated -= CheckBuyingEligibility;

		}

		public override void BuyItem()
		{
			if (_playerGuildToken.Value < Price)
			{
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
			OnMachineBuyed?.Invoke(_shopItem.MachineTemplate);
			InventoryController.Instance.AddMachineToPlayerInventory(_shopItem.MachineTemplate, 1);

		}
	}
}
