using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Components.Grid.Tile
{
    [System.Serializable]
    public class TileController : MonoBehaviour
    {
        [SerializeField] private Transform _3dViewHolder;
        [SerializeField] private TileType _tileType;
        [SerializeField] private Cell _cell;

        private GameObject _view;

		public TileType TileType => _tileType;
		public Cell Cell => _cell;


		// ------------------------------------------------------------------------- INIT -------------------------------------------------------------------------
		public void InstantiatePreview(TileTemplate tileTemplate, float scale)
        {
            _view = Instantiate(tileTemplate.GridView, _3dViewHolder);
           
            _view.transform.localScale = new Vector3(scale, scale, scale);
        }

		public void SetCell(Cell cell)
		{
			_cell = cell;
		}

        public void SetTileType(TileType tileType)
		{
            _tileType = tileType;
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
