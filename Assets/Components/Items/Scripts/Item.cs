using Components.Machines;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Components.Items
{
	public class Item
	{
		public ItemTemplate Template;
		public List<Resource> Resources { get; protected set; }
		public List<ItemType> Types { get; protected set; }
		public GameObject Item3DObject { get; protected set; }
		public string ResourcesName { get; protected set; }
		public string TypesName { get; protected set; }

		private ItemView _itemView;

		public Item ConstructFromTemplates(ItemTemplate template)
		{
			Item item = new Item();
			item.Template = template;
			item.Resources = template.Resources;
			item.Types = template.Types;
			item.Item3DObject = template.Item3DObject;
			item.TypesName = template.TypesName;
			item.ResourcesName = template.ResourcesName;

			//item._itemView = item.Item3DObject
			return item;
		}


		public Item ConstructNewItem(List<Resource> itemResources, List<ItemType> itemTypes,GameObject item3DObject)
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
