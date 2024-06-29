using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;


namespace Components.Items
{
	public class ItemView : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _resourcesNameText;
		[SerializeField] private TextMeshProUGUI _typesNameText;

		public void Init(Item item)
		{
			_resourcesNameText.text = "";
			_typesNameText.text = "";

			foreach (Resource resource in item.Resources)
			{
				_resourcesNameText.text += resource.ToString() + " ";
			}

			foreach (ItemType type in item.Resources)
			{
				_typesNameText.text += type.ToString() + " ";
			}

		}
	}
}

