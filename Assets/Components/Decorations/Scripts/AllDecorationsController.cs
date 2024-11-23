using System.Collections.Generic;
using UnityEngine;

namespace Components.Grid.Decorations
{
	[CreateAssetMenu(fileName = "New All Decoration Template", menuName = "Grid/All Decoration Template")]
	public class AllDecorationsController : ScriptableObject
    {
        [SerializeField] private List<DecorationController> _decorationsList;

		public DecorationController GenerateDecorationFromPrefab(Grid grid, Cell chosenCell, Transform decorationHolder, float cellSize, DecorationController decorationController)
		{
			DecorationController decoration = Instantiate(decorationController, decorationHolder);
			decoration.transform.position = grid.GetWorldPosition(chosenCell.X, chosenCell.Y) + new Vector3(cellSize / 2, 0, cellSize / 2);
			decoration.transform.localScale = new Vector3(cellSize, cellSize, cellSize);


			decoration.SetDecorationType(decorationController.DecorationType);
			chosenCell.AddDecorationToCell(decoration);
			return decoration;
		}

		public DecorationController GenerateDecorationFromType(Cell chosenCell, Grid grid, Transform decorationHolder, float cellSize, DecorationType decorationType)
		{
			foreach (DecorationController decorationController in _decorationsList)
			{
				if (decorationController.DecorationType == decorationType)
				{
					DecorationController decoration = Instantiate(decorationController, decorationHolder);
					decoration.transform.position = grid.GetWorldPosition(chosenCell.X, chosenCell.Y) + new Vector3(cellSize / 2, 0, cellSize / 2);
					decoration.transform.localScale = new Vector3(cellSize, cellSize, cellSize);


					decoration.SetDecorationType(decorationType);
					chosenCell.AddDecorationToCell(decoration);
					return decoration;
				}
			}

			Debug.LogError("Can't find obstacle associated ObstacleType : " + decorationType);
			return null;
		}
	}
}

