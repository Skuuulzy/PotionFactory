using Components.Machines;
using Components.Shop.ShopItems;
using Database;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Shop
{
	public class ShopController : MonoBehaviour
	{
		[SerializeField] private int _numberOfMachineItemInShop = 5;
		[SerializeField] private int _numberOfConsumableItemInShop = 3;
		[SerializeField] private int _numberOfRelicItemInShop = 1;

		private List<ShopItem> _shopItemsList;

		public static Action<List<ShopItem>> OnShopGenerated;

		private void Start()
		{
			ShopState.OnShopStateStarted += GenerateShop;
		}
		
		private void OnDestroy()
		{
			ShopState.OnShopStateStarted -= GenerateShop;
		}
		
		private void GenerateShop(ShopState state)
		{
			_shopItemsList = new List<ShopItem>();
			List<MachineTemplate> allMachinesTemplate = ScriptableObjectDatabase.GetAllScriptableObjectOfType<MachineTemplate>();
			ShopItem conveyorShopItem = new ShopItem(allMachinesTemplate.Find(x => x.Name == "Conveyor"));
			_shopItemsList.Add(conveyorShopItem);
			allMachinesTemplate.Remove(allMachinesTemplate.Find(x => x.Name == "Conveyor"));

			for (int i = 0; i < _numberOfMachineItemInShop; i++)
			{
				ShopItem shopItem = new ShopItem(allMachinesTemplate, _shopItemsList);
				_shopItemsList.Add(shopItem);
			}
			
			OnShopGenerated?.Invoke(_shopItemsList);
		}
	}
}