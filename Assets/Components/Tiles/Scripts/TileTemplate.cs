using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Components.Grid.Tile
{
	[CreateAssetMenu(fileName = "New Tile Template", menuName = "Grid/Tile Template")]
	public class TileTemplate : ScriptableObject
	{
		[Header("Definition")]
		[SerializeField] private string _name;

		[SerializeField] private GameObject _gridView;
		[SerializeField] private Sprite _uiView;

		public string Name => _name;

		public GameObject GridView => _gridView;
		public Sprite UIView => _uiView;
	}
}

