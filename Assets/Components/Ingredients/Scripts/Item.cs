using System.Collections.Generic;

namespace Components.Items
{
	public class Item
	{
		public readonly IngredientTemplate Template;
		public readonly List<Resource> Resources;
		public readonly List<ItemState> Types;

		public Item (IngredientTemplate template)
		{
			Template = template;
			Resources = template.Resources;
			Types = template.Types;
		}
		
		public Item(List<Resource> itemResources, List<ItemState> itemTypes)
		{
			Resources = itemResources;
			Types = itemTypes;
		}
	}
}