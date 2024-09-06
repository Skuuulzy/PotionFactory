using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Shop.ShopItems
{
    [CreateAssetMenu(fileName = "New Shop Item Template", menuName = "Shop/Shop Item Template")]

    public class ShopItemTemplate : ScriptableObject
    {
        // ----------------------------------------------------------------------- PRIVATE FIELDS -------------------------------------------------------------------------

        [SerializeField] private string _name;
        [SerializeField] private string _description;
        [SerializeField] private Sprite _uiView;
        [SerializeField] private float _spawnProbability;
        [SerializeField] private bool _basicShopItem;

        // ----------------------------------------------------------------------- PUBLIC FIELDS -------------------------------------------------------------------------
        public string Name => _name;
        public string Description => _description;
        public Sprite UIView => _uiView;
        public float spawnProbability => _spawnProbability;
        public bool BasicShopItem => _basicShopItem;
    }
}
