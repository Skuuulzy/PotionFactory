using System.Collections.Generic;
using UnityEngine;
using VComponent.Tools.Singletons;
namespace Components.Items
{
	public class ItemManager : Singleton<ItemManager>
	{
		[SerializeField] private SerializableDictionary<ItemType, GameObject> _itemType3DObjectsDictionary;
		[SerializeField] private SerializableDictionary<Resource, Sprite> _itemResourceSpriteDictionary;

		public GameObject GetTypeRepresantation(List<ItemType> itemType)
		{
			//Only return the first type for now
			return _itemType3DObjectsDictionary[itemType[0]];
		}

		public List<Sprite> GetResourceRepresantation(List<Resource> itemResources)
		{
			List<Sprite> spriteList = new List<Sprite>();

			foreach(Resource resource in itemResources)
			{
				spriteList.Add(_itemResourceSpriteDictionary[resource]);
			}

			return spriteList;
		}
	}
}

