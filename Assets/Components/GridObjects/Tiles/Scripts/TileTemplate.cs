using UnityEngine;

namespace Components.Grid.Tile
{
	[CreateAssetMenu(fileName = "New Tile Template", menuName = "Component/Grid/Tile Template")]
	public class TileTemplate : GridObjectTemplate
	{
		[SerializeField] private TileType _tileType;
		[SerializeField] private Material _lockedMaterial;
		[SerializeField] private Material _unlockedMaterial;

		public TileType TileType => _tileType;
		public Material LockedMaterial => _lockedMaterial;
		public Material UnlockedMaterial => _unlockedMaterial;
	}
}