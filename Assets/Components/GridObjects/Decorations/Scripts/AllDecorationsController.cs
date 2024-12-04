using System.Collections.Generic;
using UnityEngine;

namespace Components.Grid.Decorations
{
	[CreateAssetMenu(fileName = "New All Decoration Template", menuName = "Grid/All Decoration Template")]
	public class AllDecorationsController : ScriptableObject
    {
        [SerializeField] private List<DecorationController> _decorationsList;

		public DecorationController GenerateDecorationFromPrefab(Grid grid, Cell chosenCell, Transform decorationHolder, float cellSize, DecorationController decorationController, bool freePlacement, Vector3 worldMousePosition)
		{
			DecorationController decoration = Instantiate(decorationController, decorationHolder);
			if (freePlacement)
			{
				decoration.transform.position = worldMousePosition; 
			}
			else
			{
				decoration.transform.position = grid.GetWorldPosition(chosenCell.X, chosenCell.Y) + new Vector3(cellSize / 2, 0, cellSize / 2);

			}
			decoration.transform.localScale = decorationController.transform.localScale;
			decoration.transform.rotation = decorationController.transform.rotation;

			decoration.SetDecorationType(decorationController.DecorationType);
			chosenCell.AddDecorationToCell(decoration);
			return decoration;
		}

        /// <summary>
        /// Generates a decoration from a given type, position, rotation, and scale, and assigns it to the cell.
        /// </summary>
        public DecorationController GenerateDecorationFromType(Cell chosenCell, Transform decorationHolder, DecorationType decorationType, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
        {
            foreach (DecorationController decorationController in _decorationsList)
            {
                if (decorationController.DecorationType == decorationType)
                {
                    // Instantiate the decoration
                    DecorationController decoration = Instantiate(decorationController, decorationHolder);

                    // Apply position, rotation, and scale
                    decoration.transform.localPosition = localPosition;
                    decoration.transform.localRotation = localRotation;
                    decoration.transform.localScale = localScale;

                    // Set decoration type and assign to cell
                    decoration.SetDecorationType(decorationType);
                    chosenCell.AddDecorationToCell(decoration);

                    return decoration;
                }
            }

            Debug.LogError("Can't find decoration associated with DecorationType: " + decorationType);
            return null;
        }

    }
}

