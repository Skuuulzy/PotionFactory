using Components.Ingredients;
using Components.Machines.Behaviors;
using Components.Shop.ShopItems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Economie
{
	public class EconomieController : MonoBehaviour
	{

		public static  int PlayerMoney = 0;

		public static Action<int> OnPlayerMoneyUpdate;
		void Start()
		{
			DestructorMachineBehaviour.OnItemSold += OnItemsSold;
			ShopItemUIViewController.OnItemBuyed += OnItemBuyed;
		}

		private void OnDestroy()
		{
			DestructorMachineBehaviour.OnItemSold -= OnItemsSold;
			ShopItemUIViewController.OnItemBuyed -= OnItemBuyed;

		}


		private void OnItemsSold(List<IngredientTemplate> ingredientTemplates)
		{
			foreach(IngredientTemplate ingredientTemplate in ingredientTemplates)
			{
				PlayerMoney += ingredientTemplate.Price;
			}

			OnPlayerMoneyUpdate?.Invoke(PlayerMoney);
		}

		private void OnItemBuyed(int itemPrice)
		{
			PlayerMoney -= itemPrice;
			OnPlayerMoneyUpdate?.Invoke(PlayerMoney);
		}
	}
}

