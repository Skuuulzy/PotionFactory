using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Components.Grid.Tile
{
    public class TileController : MonoBehaviour
    {
        [SerializeField] private Transform _3dViewHolder;

        private GameObject _view;

        // ------------------------------------------------------------------------- INIT -------------------------------------------------------------------------
        public void InstantiatePreview(TileTemplate tileTemplate, float scale)
        {
            _view = Instantiate(tileTemplate.GridView, _3dViewHolder);
           
            _view.transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}
