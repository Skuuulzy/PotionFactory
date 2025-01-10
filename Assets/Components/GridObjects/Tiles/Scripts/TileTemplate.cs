using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Components.Grid.Tile
{
	[CreateAssetMenu(fileName = "New Tile Template", menuName = "Component/Grid/Tile Template")]
	public class TileTemplate : GridObjectTemplate
	{
		[SerializeField] private TileType _tileType;
		public TileType TileType => _tileType;

	}
}

