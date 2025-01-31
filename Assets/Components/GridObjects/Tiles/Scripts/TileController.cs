using UnityEngine;

namespace Components.Grid.Tile
{
    public class TileController : GridObjectController
    {
        [SerializeField] private TileType _tileType;
        
        public TileType TileType => _tileType;
        
        private TileTemplate TileTemplate => Template as TileTemplate;

        // ------------------------------------------------------------------------- INIT -------------------------------------------------------------------------		
        public void SetTileType(TileType tileType)
        {
            _tileType = tileType;
        }
        
        public void SetLockedState(bool locked)
        {
            if (View.TryGetComponent(out MeshRenderer meshRenderer))
            {
                meshRenderer.material = locked ? TileTemplate.LockedMaterial : TileTemplate.UnlockedMaterial;
            }
        }

        // TODO: Convert the getter to this
        public TileType GetTileType()
        {
            return TileTemplate.TileType;
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