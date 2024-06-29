using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Components.Items
{
	public class Item : MonoBehaviour
	{
		public List<Resource> Resources { get; protected set; }
		public List<ItemType> Types { get; protected set; }
		public GameObject Item3DObject { get; protected set; }
		public string ResourcesName { get; protected set; }
		public string TypesName { get; protected set; }

		public Item ConstructFromTemplates(ItemTemplate template)
		{
			Item item = new Item();
			Resources = template.Resources;
			Types = template.Types;
			Item3DObject = template.Item3DObject;
			TypesName = template.TypesName;
			ResourcesName = template.ResourcesName;

			return item;
		}


		public Item ConstructNewItem(List<Resource> itemResources, List<ItemType> itemTypes,GameObject item3DObject, string itemResourcesName, string itemTypesName)
		{
			Item item = new Item();
			item.Resources = itemResources;
			item.Types = itemTypes;
			item.Item3DObject = item3DObject;
			item.ResourcesName = itemResourcesName;
			item.TypesName = itemTypesName;

			return item;
		}

	}
}
