using Components.Bundle;
using Components.Ingredients;
using Components.Machines;
using Components.Shop.ShopItems;
using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Components.Shop
{
	public class UIShopController : MonoBehaviour
	{

		[Header("Shop")]
		[SerializeField] private GameObject _shopUIView;
		[SerializeField] private UIMachineShopItemViewController _machineShopUIViewController;
		[SerializeField] private UIIngredientShopItemViewController _ingredientShopUIViewController;
		[SerializeField] private UIBuyedItemController _buyedItemController;

		[SerializeField] private Transform _machineShopUIParent;
		[SerializeField] private Transform _ingredientShopUIParent;
		[SerializeField] private Transform _itemsBuyedParent;

		private void Awake()
		{
			ShopController.OnShopGenerated += SetInfos;
			UIMachineShopItemViewController.OnMachineBuyed += AddMachineToBuyedItems;
            ResolutionFactoryState.OnResolutionFactoryStateStarted += HandleResolutionState;
		}

        private void OnDestroy()
		{
			ShopController.OnShopGenerated -= SetInfos;
		}

		private void SetInfos(List<ShopItem> shopItems)
		{

			//Destroying all child before instantiate new ones
			foreach (Transform child in _machineShopUIParent)
			{
				Destroy(child.gameObject);
			}

			//Instantiate all shop items
			foreach (ShopItem shopItem in shopItems)
			{
				//Check if the shopItem contains a machine
				if (shopItem.MachineTemplate != null)
				{
					UIMachineShopItemViewController UIMachineShopItemViewController = Instantiate(_machineShopUIViewController, _machineShopUIParent);
					UIMachineShopItemViewController.Init(shopItem);
				}
			}

		}

        private void HandleResolutionState(ResolutionFactoryState state)
        {
            var startingRoundIngredientsBundles = ScriptableObjectDatabase.GetAllScriptableObjectOfType<IngredientTemplate>().Where(ingredient => ingredient.NumberOfTransformation == 0).ToList();
			startingRoundIngredientsBundles = startingRoundIngredientsBundles.OrderBy(_ => UnityEngine.Random.value).ToList();

            //Destroying all child before instantiate new ones
            foreach (Transform child in _ingredientShopUIParent)
            {
                Destroy(child.gameObject);
            }

			for(int i = 0; i < 3; i++)
			{
				ShopItem shopItem = new ShopItem(startingRoundIngredientsBundles[i]);
                UIIngredientShopItemViewController ingredient = Instantiate(_ingredientShopUIViewController, _ingredientShopUIParent);
                ingredient.Init(shopItem);
            }

        }

        public void OpenShop(bool open)
		{
			_shopUIView.SetActive(open);
			
			//Destroying all child before instantiate new ones
			foreach (Transform child in _itemsBuyedParent)
			{
				Destroy(child.gameObject);
			}
		}

		public void OnOpenShop()
		{
			//Destroying all child before instantiate new ones
			foreach (Transform child in _itemsBuyedParent)
			{
				Destroy(child.gameObject);
			}
		}

		private void AddMachineToBuyedItems(MachineTemplate template)
		{
			var newItem = Instantiate(_buyedItemController, _itemsBuyedParent);
			newItem.SetInfos(template);
		}
	}
}
