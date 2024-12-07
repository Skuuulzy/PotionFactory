using CodeMonkey.Utils;
using UnityEngine;

namespace Components.Grid
{
    // TODO: Maybe merge this class with the preview controller and separate instantiation and selection ?
    public class GridSelector : MonoBehaviour
    {
        [SerializeField] private GridController _gridController;

        private Grid Grid => _gridController.Grid;

        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                TrySelectMachine();
            }
        }

        private void TrySelectMachine()
        {
            // TODO: This click workflow really need to be centralized (maybe in the input sys)
            // Try to get the position on the grid. 
            if (!UtilsClass.ScreenToWorldPositionIgnoringUI(Input.mousePosition, _camera, out Vector3 worldMousePosition))
            {
                return;
            }
            
            // Try getting the cell 
            if (!Grid.TryGetCellByPosition(worldMousePosition, out Cell chosenCell))
            {
                return;
            }

            if (chosenCell.ContainsNode)
            {
                chosenCell.Node.Machine.Select();
            }
        }
    }
}