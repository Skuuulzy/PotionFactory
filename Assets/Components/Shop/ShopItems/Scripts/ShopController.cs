using Components.Machines;
using Components.Shop.ShopItems;
using Database;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Components.Shop
{
	public class ShopController : MonoBehaviour
	{
		[SerializeField] private int _numberOfConsumableItemInShop = 3;
		[SerializeField] private int _numberOfRelicItemInShop = 2;
		
		public static Action<List<ShopItem>> OnShopGenerated;

		private void Start()
		{
			ResolutionFactoryState.OnStateStarted += GenerateShop;
		}
		
		private void OnDestroy()
		{
			ResolutionFactoryState.OnStateStarted -= GenerateShop;
		}

		private void GenerateShop(BaseState state)
		{
			List<ShopItem> shopItemListToGenerate = new List<ShopItem>();
			shopItemListToGenerate.AddRange(GenerateMachinesInShop());
			//ShopIsGenerated
			OnShopGenerated?.Invoke(shopItemListToGenerate);
			ResolutionFactoryState.OnStateStarted -= GenerateShop;
		}

		private List<ShopItem> GenerateMachinesInShop()
		{
			//Generate allItemList to select random items from it and the shopItemList to generate in shop
			List<ShopItem> allMachineShopItemList = new List<ShopItem>();
			List<ShopItem> shopItemListToGenerate = new List<ShopItem>();

			//Get all existing machines
			List<MachineTemplate> allMachinesTemplate = ScriptableObjectDatabase.GetAllScriptableObjectOfType<MachineTemplate>().FindAll(x => !x.CannotBeBuy);


			for (int i = 0; i < allMachinesTemplate.Count; i++)
			{
				ShopItem shopItem = new ShopItem(allMachinesTemplate[i], -1);
				allMachineShopItemList.Add(shopItem);
			}

			return allMachineShopItemList;
		}


		/// <summary>
		/// Selects a random item from the list based on spawn probability.
		/// </summary>
		public List<ShopItem> GetRandomItemList(int count, List<ShopItem> itemList)
		{
			// Make a copy of the items list to avoid modifying the original
			List<ShopItem> availableItems = itemList;
			List<ShopItem> selectedItems = new List<ShopItem>();


			for (int i = 0; i < count; i++)
			{
				if (availableItems.Count == 0) break;

				// Select a random item based on probability
				ShopItem selectedItem = GetRandomItem(availableItems);

				// Add to the selected list and remove from the available list
				selectedItems.Add(selectedItem);
				availableItems.Remove(selectedItem);
			}

			return selectedItems;
		}

		/// <summary>
		/// Selects a random item from a given list based on spawn probability.
		/// </summary>
		/// <param name="availableItems">The list of items to select from.</param>
		/// <returns>Selected Item based on probability.</returns>
		private ShopItem GetRandomItem(List<ShopItem> availableItems)
		{
			float totalWeight = GetTotalWeight(availableItems);
			float randomValue = UnityEngine.Random.value * totalWeight;

			return GetItemBasedOnProbability(randomValue, availableItems);
		}

		/// <summary>
		/// Sums the spawn probabilities of all items in the given list.
		/// </summary>
		/// <param name="availableItems">The list of items to sum probabilities for.</param>
		/// <returns>Total weight of all spawn probabilities.</returns>
		private float GetTotalWeight(List<ShopItem> availableItems)
		{
			float total = 0;
			foreach (ShopItem item in availableItems)
			{
				total += item.SpawnProbability;
			}
			return total;
		}

		/// <summary>
		/// Chooses an item from the list based on the accumulated weight and random value.
		/// </summary>
		/// <param name="randomValue">A random value multiplied by the total weight.</param>
		/// <param name="availableItems">The list of items to select from.</param>
		/// <returns>Selected Item based on probability.</returns>
		private ShopItem GetItemBasedOnProbability(float randomValue, List<ShopItem> availableItems)
		{
			float cumulativeWeight = 0;

			foreach (ShopItem item in availableItems)
			{
				cumulativeWeight += item.SpawnProbability;
				if (randomValue <= cumulativeWeight)
				{
					return item;
				}
			}

			return null; // In case of no match (shouldn't happen if probabilities are well-formed)
		}
	}
}