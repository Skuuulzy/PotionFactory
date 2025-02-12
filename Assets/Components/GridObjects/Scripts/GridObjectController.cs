using UnityEngine;

namespace Components.Grid
{
	public abstract class GridObjectController : MonoBehaviour
	{
		[SerializeField] protected Transform _3dViewHolder;

		protected bool Instanced;
		protected GameObject View;
		protected GridObjectTemplate Template;
		
		public static GridObjectController InstantiateFromTemplate(GridObjectTemplate template, float scale, Transform parent)
		{
			var gridObjectController = Instantiate(template.GridObjectControllerPrefab, parent);
			gridObjectController.InstantiatePreview(template, scale);
			
			return gridObjectController;
		}
		
		public static GridObjectController InstantiateAndAddToGridFromTemplate(GridObjectTemplate template, Cell cell, Grid grid, Transform parent)
		{
			var gridObjectController = Instantiate(template.GridObjectControllerPrefab, parent);
			gridObjectController.InstantiatePreview(template, grid.GetCellSize());
			gridObjectController.AddToGrid(cell, grid, parent);
			
			return gridObjectController;
		}
		
		protected virtual void InstantiatePreview(GridObjectTemplate template, float scale)
		{
			View = Instantiate(template.GridView, _3dViewHolder);
			Template = template;

			transform.localScale = new Vector3(scale, scale, scale);

			Instanced = true;
		}
		
		public virtual void AddToGrid(Cell originCell, Grid grid, Transform parent)
		{
			if (!Instanced)
			{
				Debug.LogError("You try to add an object to the grid that is not yet instanced. Call Instantiate preview before Adding to grid.");
				return;
			}
			
			var cellSize = grid.GetCellSize();
			
            transform.position = grid.GetWorldPosition(originCell.X, originCell.Y) + new Vector3(cellSize / 2, 0, cellSize / 2);
            transform.name = $"{Template.Name}_{parent.childCount}";
            transform.parent = parent;
        }
	}
}