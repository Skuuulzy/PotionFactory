using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Components.Grid.Tile
{
    public class TileController : MonoBehaviour
    {
        [SerializeField] private Transform _3dViewHolder;

        private GameObject _view;
        private int[] _tileCoordinate;
        private bool _isWater;

        public int[] TileCoordinate => _tileCoordinate;
        public bool IsWater => _isWater;



        // ------------------------------------------------------------------------- INIT -------------------------------------------------------------------------
        public void InstantiatePreview(TileTemplate tileTemplate, float scale)
        {
            _view = Instantiate(tileTemplate.GridView, _3dViewHolder);
           
            _view.transform.localScale = new Vector3(scale, scale, scale);
        }

		public void SetCoordinate(int x, int z)
		{
			_tileCoordinate = new int[2] { x, z };
		}

        public void TileIsWater(bool isWater)
		{
            _isWater = isWater;
		}
	}
}
