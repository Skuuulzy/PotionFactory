using Components.Grid.Tile;
using UnityEngine;

namespace Components.Grid
{
	public abstract class GridObjectController : MonoBehaviour
	{
		[SerializeField] protected Transform _3dViewHolder;

		protected GameObject View;
		protected GridObjectTemplate Template;
		
		// ------------------------------------------------------------------------- INIT -------------------------------------------------------------------------
		public virtual void InstantiatePreview(GridObjectTemplate template, float scale)
		{
			View = Instantiate(template.GridView, _3dViewHolder);

			transform.localScale = new Vector3(scale, scale, scale);
		}

		public virtual void InstantiateOnGrid(GridObjectTemplate template, Vector3 cellWorldPosition, float cellSize, Transform parent)
		{			
			View = Instantiate(template.GridView, _3dViewHolder);
			Template = template;
			
			transform.parent = parent;
			transform.position = cellWorldPosition + new Vector3(cellSize / 2, 0, cellSize / 2);
			transform.localScale = new Vector3(cellSize, cellSize, cellSize);
			transform.name = $"{Template.Name} ({cellWorldPosition.x}, {cellWorldPosition.y})";
		}
	}
}