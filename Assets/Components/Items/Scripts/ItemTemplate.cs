
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Components.Items
{
	[CreateAssetMenu(fileName = "New Item Template", menuName = "Machines/Item Template")]
	public class ItemTemplate : ScriptableObject
	{

		[SerializeField] private List<Resource> _resources;
		[SerializeField] private List<ItemType> _types;
		[SerializeField] private GameObject _3dView;

		public List<Resource> Resources => _resources;
		public List<ItemType> Types => _types;
		public GameObject View => _3dView;

	}
}

