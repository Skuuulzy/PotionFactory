using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Components.Items
{
	public class ItemController : MonoBehaviour
	{
		[SerializeField] private Transform _3dViewHolder;
		[SerializeField] private Item _item;

		public Item Item => _item;

		public void Init(ItemTemplate itemTemplate)
		{
			_item = new Item(itemTemplate);

			Instantiate(_item.Template.View, _3dViewHolder);

		}

		public void Init(List<Resource> resources, List<ItemType> itemTypes)
		{
			_item = new Item(resources, itemTypes);
			Item3DView item3DView = ItemManager.Instance.GetTypeRepresantation(itemTypes[0], resources);
			Instantiate(item3DView, _3dViewHolder);
		}
	}
}
	
