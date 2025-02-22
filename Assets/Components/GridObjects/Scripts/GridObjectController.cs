using UnityEngine;

namespace Components.Grid
{
	public abstract class GridObjectController : MonoBehaviour
	{
		[SerializeField] protected Transform _3dViewHolder;

		protected bool Instanced;
		protected GameObject ViewGO;
		protected GridObjectViewController GridObjectView;
		protected GridObjectTemplate Template;
		
		// ----------------------------------------- STATIC METHODS ------------------------------------------
		
		public static GridObjectController InstantiateFromTemplate(GridObjectTemplate template, float scale, Transform parent)
		{
			var gridObjectController = Instantiate(template.GridObjectControllerPrefab, parent);
			
			// Instantiating View
			Vector3 localScale = new Vector3(scale, scale, scale);
			gridObjectController.InstantiateView(template, Quaternion.identity, localScale);
			
			return gridObjectController;
		}
		
		public static GridObjectController InstantiateAndAddToGridFromTemplate(GridObjectTemplate template, Cell cell, Grid grid, Transform parent)
		{
			var gridObjectController = Instantiate(template.GridObjectControllerPrefab, parent);

			// Instantiating View
			Vector3 localScale = new Vector3(grid.GetCellSize(), grid.GetCellSize(), grid.GetCellSize());
			gridObjectController.InstantiateView(template, Quaternion.identity, localScale);
			
			// Place view on grid
			gridObjectController.AddToGrid(cell, grid, parent);
			
			return gridObjectController;
		}

		public static GridObjectController InstantiateAndAddToGridFromTemplate(GridObjectTemplate template, Cell cell, Grid grid,Transform parent, Quaternion localRotation, Vector3 localScale)
		{
			var gridObjectController = Instantiate(template.GridObjectControllerPrefab, parent);
			gridObjectController.InstantiateView(template, localRotation, localScale);
			gridObjectController.AddToGrid(cell, grid, parent);
			
			return gridObjectController;
		}
		
		// ----------------------------------------- VIRTUAL METHODS ------------------------------------------
		
		protected virtual void InstantiateView(GridObjectTemplate template, Quaternion localRotation, Vector3 localScale)
		{
			if (!template.GridView)
			{
				Debug.LogError($"No grid view on template: {template.name}");
				return;
			}
			
			ViewGO = Instantiate(template.GridView, _3dViewHolder);
			if (ViewGO.TryGetComponent(out GridObjectViewController gridObjectController))
			{
				GridObjectView = gridObjectController;
			}
			Template = template;

			transform.localScale = localScale;
			transform.localRotation = localRotation;

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

			transform.position = grid.GetWorldPosition(originCell.Coordinates) + new Vector3(cellSize / 2, 0, cellSize / 2);
            transform.parent = parent;
        }

		public void UpdateGridViewPlacableState(bool isMachinePlacable)
		{
			if (GridObjectView)
			{
				GridObjectView.UpdateBlueprintColor(isMachinePlacable);
			}
		}
	}
}