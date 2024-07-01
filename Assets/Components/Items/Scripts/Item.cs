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

		public Item (ItemTemplate template)
		{
			Template = template;
			Resources = template.Resources;
			Types = template.Types;
		}


		public Item(List<Resource> itemResources, List<ItemType> itemTypes)
		{
			Resources = itemResources;
			Types = itemTypes;
		}

		

	}
}
