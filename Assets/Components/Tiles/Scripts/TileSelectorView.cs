using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Components.Grid.Tile
{
    public class TileSelectorView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _name;
        [SerializeField] private Image _background;
        public Action<TileTemplate> OnSelected;

        private TileTemplate _tile;

        public void Init(TileTemplate tile)
        {
            _tile = tile;

            _name.text = tile.Name;
            _background.sprite = tile.UIView;
        }

        public void Select()
        {
            OnSelected?.Invoke(_tile);
        }
    }
}

