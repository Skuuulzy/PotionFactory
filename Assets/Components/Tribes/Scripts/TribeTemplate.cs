using Components.Shop.ShopItems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Tribes
{
    [CreateAssetMenu(fileName = "New Tribe Template", menuName = "Tribes/Tribe Template")]

    public class TribeTemplate : ScriptableObject
    {
        // ----------------------------------------------------------------------- PRIVATE FIELDS -------------------------------------------------------------------------
        [Header("Definition")]
        [SerializeField] private string _name;
        [SerializeField] private Sprite _uiView;

        // ----------------------------------------------------------------------- PUBLIC FIELDS -------------------------------------------------------------------------
        public string Name => _name;
        public Sprite UIView => _uiView;

    }
}
