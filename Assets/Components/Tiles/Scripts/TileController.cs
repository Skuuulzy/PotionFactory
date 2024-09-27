using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Components.Grid.Tile
{
    public class TileController : MonoBehaviour
    {
        [SerializeField] private Transform _3dViewHolder;
        [SerializeField] private TileType _tileType;

        private GameObject _view;

		public TileType TileType => _tileType;


		// ------------------------------------------------------------------------- INIT -------------------------------------------------------------------------
		public void InstantiatePreview(TileTemplate tileTemplate, float scale)
        {
            _view = Instantiate(tileTemplate.GridView, _3dViewHolder);
           
            transform.localScale = new Vector3(scale, scale, scale);
        }
		
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
