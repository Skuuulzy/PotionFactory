using Components.Ingredients;
using Components.Machines.Behaviors;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Economie
{
	public class EconomieController : MonoBehaviour
	{

		private int _playerMoney = 0;

		public static Action<int> OnPlayerMoneyUpdate;
		void Start()
		{
			DestructorMachineBehaviour.OnItemSold += OnItemsSold;
		}

		private void OnDestroy()
		{
			DestructorMachineBehaviour.OnItemSold -= OnItemsSold;
		}


		private void OnItemsSold(List<IngredientTemplate> ingredientTemplates)
		{
			foreach(IngredientTemplate ingredientTemplate in ingredientTemplates)
			{
				_playerMoney += ingredientTemplate.Price;
			}

			OnPlayerMoneyUpdate?.Invoke(_playerMoney);
		}
	}
}

