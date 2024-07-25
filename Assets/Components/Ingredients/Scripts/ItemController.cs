using System.Collections.Generic;
using UnityEngine;

namespace Components.Items
{
	public class ItemController : MonoBehaviour
	{
		[SerializeField] private Transform _3dViewHolder;
		
		private Item _item;
		private Item3DView _itemInstance; 
		
		public void CreateRepresentationFromTemplate(IngredientTemplate ingredientTemplate)
		{
			_item = new Item(ingredientTemplate);
			Instantiate(_item.Template.View, _3dViewHolder);
		}

		public void CreateRepresentationWith(List<Resource> resources, List<ItemState> itemTypes)
		{
			if(_itemInstance)
			{
				DestroyRepresentation();
			}
			
			_item = new Item(resources, itemTypes);
			_itemInstance = Instantiate(ItemManager.Instance.GetTemplateWith(itemTypes[0], resources), _3dViewHolder);
		}

		public void DestroyRepresentation()
		{
			if (!_itemInstance)
			{
				return;
			}
			
			_item = null;
			Destroy(_itemInstance.gameObject);
		}
	}
}