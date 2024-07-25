using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Components.Grid.Tile
{
    [System.Serializable]
    public class TileController : MonoBehaviour
    {
        [SerializeField] private Transform _3dViewHolder;
        [SerializeField] public TileType TileType;
        [SerializeField] public int TileCoordinateX;
        [SerializeField] public int TileCoordinateZ;

        private GameObject _view;





        // ------------------------------------------------------------------------- INIT -------------------------------------------------------------------------
        public void InstantiatePreview(TileTemplate tileTemplate, float scale)
        {
            _view = Instantiate(tileTemplate.GridView, _3dViewHolder);
           
            _view.transform.localScale = new Vector3(scale, scale, scale);
        }

		public void SetCoordinate(int x, int z)
		{
            TileCoordinateX = x;
            TileCoordinateZ = z;
		}

        public void SetTileType(TileType tileType)
		{
            TileType = tileType;
		}
	}

    public enum TileType
	{
        GRASS,
        SAND,
        STONE,
        DIRT,
        WATER
	}
}
