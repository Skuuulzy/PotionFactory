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

		public Item (ItemTemplate template)
		{
			Template = template;
			Resources = template.Resources;
			Types = template.Types;
			Item3DObject = template.Item3DObject;
			TypesName = template.TypesName;
			ResourcesName = template.ResourcesName;

		}


		public Item(List<Resource> itemResources, List<ItemType> itemTypes)
		{
			Resources = itemResources;
			Types = itemTypes;
		}

		

	}
}
