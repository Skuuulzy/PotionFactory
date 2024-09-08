using Components.Economy;
using Components.Inventory;

namespace Components.Shop.ShopItems
{
    public class MachineShopItemUIViewController : ShopItemUIViewController
    {
        public override void Init(ShopItem shopItem)
        {
            base.Init(shopItem);
            _itemName.text = shopItem.MachineTemplate.Name;
            _itemPrice.text = $"{shopItem.MachineTemplate.ShopPrice}";
            _itemView.sprite = shopItem.MachineTemplate.UIView;
            Price = shopItem.MachineTemplate.ShopPrice;

		}

		public override void BuyItem()
		{
			if (EconomyController.Instance.PlayerMoney < Price)
				return;

			EconomyController.Instance.RemoveMoney(Price);

			_soldItemGO.SetActive(true);
			InventoryController.Instance.AddMachineToPlayerInventory(_shopItem.MachineTemplate, 1);

		}
	}
}
