using System;
using CodeMonkey.Utils;
using Components.Machines;
using UnityEngine;

namespace Components.Grid
{
    // TODO: Maybe merge this class with the preview controller and separate instantiation and selection ?
    public class GridSelector : MonoBehaviour
    {
        [SerializeField] private GridController _gridController;

        private Grid Grid => _gridController.Grid;
        private Camera _camera;

        private Machine _hoveredMachine;
        private bool _initialized;
        private bool _inPreview;

        private void Start()
        {
            _camera = Camera.main;
            GridController.OnGridGenerated += HandleGridGenerated;
            GridPreviewController.OnPreview += HandlePreview;
        }

        private void Update()
        {
            if (!_initialized || _inPreview)
            {
                return;
            }
            
            TryHoverMachine();
            
            return;
            if (Input.GetMouseButtonUp(0))
            {
                TrySelectMachine();
            }
            if (Input.GetMouseButtonDown(1))
            {
                Machine.OnSelected?.Invoke(null, false);
            }
        }

        private void OnDestroy()
        {
            GridController.OnGridGenerated -= HandleGridGenerated;
            GridPreviewController.OnPreview -= HandlePreview;
        }

        private void HandleGridGenerated()
        {
            _initialized = true;
        }
        
        private void HandlePreview(bool preview)
        {
            _inPreview = preview;
        }
        
        private void TryHoverMachine()
        {
            // TODO: This click workflow really need to be centralized (maybe in the input sys)
            // Try to get the position on the grid. 
            if (!UtilsClass.ScreenToWorldPositionIgnoringUI(Input.mousePosition, _camera, out var worldMousePosition))
            {
                return;
            }
            
            // Try getting the cell 
            if (!Grid.TryGetCellByPosition(worldMousePosition, out var chosenCell))
            {
                return;
            }

            if (!chosenCell.ContainsNode)
            {
                // Un hover previous machine if it exists
                _hoveredMachine?.Hover(false);
                _hoveredMachine = null;
                
                return;
            }

            if (_hoveredMachine == chosenCell.Node.Machine)
            {
                return;
            }

            // Hover current machine
            _hoveredMachine = chosenCell.Node.Machine;
            _hoveredMachine.Hover(true);
        }

        private void TrySelectMachine()
        {
            // TODO: This click workflow really need to be centralized (maybe in the input sys)
            // Try to get the position on the grid. 
            if (!UtilsClass.ScreenToWorldPositionIgnoringUI(Input.mousePosition, _camera, out var worldMousePosition))
            {
                return;
            }
            
            // Try getting the cell 
            if (!Grid.TryGetCellByPosition(worldMousePosition, out var chosenCell))
            {
                return;
            }

            if (chosenCell.ContainsNode)
            {
                Debug.Log($"Move machine { chosenCell.Node.Machine.Controller.name}");
                //chosenCell.Node.Machine.Select(true);
                chosenCell.Node.Machine.Controller.Move();
            }
        }
    }
}