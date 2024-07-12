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
		private Item3DView _item3DView; 
		public Item Item => _item;

		private Item3DView temporaryItem;
		public void Init(ItemTemplate itemTemplate)
		{
			_item = new Item(itemTemplate);

			Instantiate(_item.Template.View, _3dViewHolder);

		}

		public void Init(List<Resource> resources, List<ItemState> itemTypes)
		{
			_item = new Item(resources, itemTypes);
			_item3DView = new Item3DView();
			_item3DView = Instantiate(ItemManager.Instance.GetTypeRepresantation(itemTypes[0], resources), _3dViewHolder);
		}

		public void DestructItem()
		{
			Destroy(_item3DView);
		}
	}
}
	
