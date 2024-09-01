using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Shop.ShopItems
{
    public class ShopItem : MonoBehaviour
    {
        // ----------------------------------------------------------------------- PRIVATE FIELDS -------------------------------------------------------------------------

        [SerializeField] private ShopItemTemplate _template;

        // ----------------------------------------------------------------------- PUBLIC FIELDS -------------------------------------------------------------------------
        public ShopItemTemplate Template => _template;
    }
}
