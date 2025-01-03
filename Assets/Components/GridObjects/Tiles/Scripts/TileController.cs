using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Components.Grid.Tile
{
    public class TileController : GridObjectController
    {
        [SerializeField] private TileType _tileType;


		public TileType TileType => _tileType;


		// ------------------------------------------------------------------------- INIT -------------------------------------------------------------------------		
        public void SetTileType(TileType tileType)
		{
            _tileType = tileType;
		}
	}

    public enum TileType
	{
        NONE,
        GRASS,
        SAND,
        STONE,
        DIRT,
        WATER
	}
}
