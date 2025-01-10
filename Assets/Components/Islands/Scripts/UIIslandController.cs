using Components.Bundle;
using Components.Map;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Components.Island
{
	public class UIIslandController : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _islandNameText;
		[SerializeField] private LevelNode[] _levelNodeList;

		public LevelNode[] LevelNodeList => _levelNodeList;
		public LevelNode StartingLevelNode => _levelNodeList[0];
		public int NumberOfNodes => _levelNodeList.Length;


		public void Init(IngredientsBundle[] ingredientBundle)
		{

			for (int i = 0; i < ingredientBundle.Length; i++)
			{
				_levelNodeList[i].Initialize(ingredientBundle[i]);
			}
		}

		public void SetIslandName(string islandName)
		{
			_islandNameText.text = $"{islandName}";
		}
	}
	
}
