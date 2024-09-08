

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
    }
}
