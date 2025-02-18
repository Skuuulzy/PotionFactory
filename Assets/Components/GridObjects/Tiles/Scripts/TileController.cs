using UnityEngine;

namespace Components.Grid.Tile
{
    public class TileController : GridObjectController
    {
        [SerializeField] private TileType _tileType;
        
        public TileType TileType => _tileType;
        private TileTemplate TileTemplate => Template as TileTemplate;

        // ------------------------------------------------------------------------- INIT -------------------------------------------------------------------------		

        protected override void InstantiateView(GridObjectTemplate template, Quaternion localRotation, Vector3 localScale)
        {
            base.InstantiateView(template, localRotation, localScale);
            
            if (template is TileTemplate tileTemplate)
            {
                _tileType = tileTemplate.TileType;
            }
        }
        
        public void SetLockedState(bool locked)
        {
            if (View.TryGetComponent(out MeshRenderer meshRenderer))
            {
                meshRenderer.material = locked ? TileTemplate.LockedMaterial : TileTemplate.UnlockedMaterial;
            }
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