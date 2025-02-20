using Components.Economy;
using Components.Ingredients;
using Components.Inventory;
using SoWorkflow.SharedValues;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Components.Shop.ShopItems
{
    public class UIIngredientShopItemViewController : UIShopItemViewController
    {
        [SerializeField] private SOSharedInt _playerGuildToken;
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
            OnIngredientBuyed?.Invoke(_shopItem.IngredientTemplate);
        }
    }
}
