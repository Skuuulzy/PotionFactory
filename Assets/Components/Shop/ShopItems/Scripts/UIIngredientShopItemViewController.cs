using Components.Economy;
using Components.Ingredients;
using Components.Inventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Components.Shop.ShopItems
{
    public class UIIngredientShopItemViewController : UIShopItemViewController
    {
        public static Action<IngredientTemplate> OnIngredientBuyed;
        public override void Init(ShopItem shopItem)
        {
            base.Init(shopItem);

            //Set parameters
            _itemName.text = shopItem.IngredientTemplate.Name;
            _itemPrice.text = $"5"; // Need to change it after discussion if we keep them in shop
            _itemView.sprite = shopItem.IngredientTemplate.Icon;
            Price = 5;
            _numberOfItemToSellText.text = shopItem.NumberOfItemToSell == -1 ? "\u221E" : $"{shopItem.NumberOfItemToSell}";

            CheckBuyingEligibility(EconomyController.Instance.PlayerMoney);
            EconomyController.OnPlayerMoneyUpdated += CheckBuyingEligibility;
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
            OnIngredientBuyed?.Invoke(_shopItem.IngredientTemplate);
        }
    }
}
