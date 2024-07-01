using System.Collections.Generic;
using UnityEngine;
using VComponent.Tools.Singletons;
namespace Components.Items
{
	public class ItemManager : Singleton<ItemManager>
	{
		[SerializeField] private SerializableDictionary<ItemType, Item3DView> _itemType3DObjectsDictionary;
		[SerializeField] private SerializableDictionary<Resource, Sprite> _itemResourceSpriteDictionary;

		public Item3DView GetTypeRepresantation(ItemType itemType, List<Resource> itemResources)
		{
			Item3DView item3DView = _itemType3DObjectsDictionary[itemType];
			List<Sprite> sprites = new List<Sprite>();
			foreach(Resource resource in itemResources)
			{
				sprites.Add(_itemResourceSpriteDictionary[resource]);
			}
			item3DView.SetSprites(sprites);
			//Only return the first type for now
			return item3DView;
		}

	}
}

