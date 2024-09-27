using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Grid.Tile
{
    public class TileManager : MonoBehaviour
    {
        [Header("Templates")]
        [SerializeField] private List<TileTemplate> _tileTemplateList;
        [Header("Selector View")]
        [SerializeField] private TileSelectorView _tileSelectorView;
        [SerializeField] private Transform _tileSelectorViewHolder;


        public TileTemplate SelectedTile { get; private set; }

        public static Action<TileTemplate> OnChangeSelectedTile;
        // Start is called before the first frame update
        void Start()
        {
            if (_tileTemplateList.Count <= 0)
            {
                Debug.LogWarning("[Tile] No templates found.");
                return;
            }

            foreach (var tile in _tileTemplateList)
            {
                TileSelectorView selectorView = Instantiate(_tileSelectorView, _tileSelectorViewHolder);
                selectorView.Init(tile);
                selectorView.OnSelected += HandleTileSelected;
            }

            // Init selected machine has the first 
            SelectedTile = _tileTemplateList[0];
        }

        private void HandleTileSelected(TileTemplate tile)
        {
            SelectedTile = tile;
            OnChangeSelectedTile?.Invoke(tile);
        }


    }
}

