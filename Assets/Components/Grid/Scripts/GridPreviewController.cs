using CodeMonkey.Utils;
using Components.Inventory;
using Components.Machines;
using UnityEngine;

namespace Components.Grid
{
    public class GridPreviewController : MonoBehaviour
    {
        [Header("Movement Parameters")] 
        [SerializeField] private bool _snapping;
        
        [Header("Components")]
        [SerializeField] private GridController _gridController;
        [SerializeField] private MachineController _machineControllerPrefab;

        
        private UnityEngine.Camera _camera;
        
        private MachineController _currentMachinePreview;
        private int _currentRotation;
        
        private bool _isFactoryState = true;

        
        private Grid Grid => _gridController.Grid;
        
        // ------------------------------------------------------------------------- MONO -------------------------------------------------------------------------------- 
        private void Start()
        {
            _camera = UnityEngine.Camera.main;
            
            MachineManager.OnChangeSelectedMachine += InstantiatePreview;
            PlanningFactoryState.OnPlanningFactoryStateStarted += HandlePlanningFactoryState;
            ShopState.OnShopStateStarted += HandleShopState;
        }

        private void Update()
        {
            //Can't interact with anything if we are not in factory state 
            if (!_isFactoryState)
            {
                return;
            }
            
            MovePreview();

            if (Input.GetMouseButton(1))
            {
                DestroyPreview();
            }
            if (Input.GetMouseButton(0))
            {
                AddSelectedMachineToGrid();
            }
            if (Input.GetMouseButtonDown(2))
            {
                RotatePreview();
            }
        }
        
        private void OnDestroy()
        {
            MachineManager.OnChangeSelectedMachine -= InstantiatePreview;
            PlanningFactoryState.OnPlanningFactoryStateStarted -= HandlePlanningFactoryState;
            ShopState.OnShopStateStarted -= HandleShopState;
        }
        
        // ------------------------------------------------------------------------- PREVIEW BEHAVIOUR -------------------------------------------------------------------------------- 
        private void InstantiatePreview(MachineTemplate template)
        {
            DestroyPreview();
            
            _currentMachinePreview = Instantiate(_machineControllerPrefab);
            _currentMachinePreview.InstantiatePreview(template, Grid.GetCellSize());
            _currentMachinePreview.RotatePreview(_currentRotation);
        }

        private void MovePreview()
        {
            if (!_currentMachinePreview)
            {
                return;
            }

            if (!UtilsClass.ScreenToWorldPositionIgnoringUI(Input.mousePosition, _camera, out Vector3 worldMousePosition))
            {
                return;
            }

            // Update the object's position 
            if (_snapping)
            {
                if (Grid.TryGetCellByPosition(worldMousePosition, out Cell cell))
                {
                    _currentMachinePreview.transform.position = cell.GetCenterPosition(_gridController.OriginPosition);
                }
            }
            else
            {
                _currentMachinePreview.transform.position = worldMousePosition;
            }
        }
        
        private void RotatePreview()
        {
            if (!_currentMachinePreview)
            {
                return;
            }

            _currentRotation += 90;
            _currentRotation %= 360;

            if (_currentMachinePreview != null)
            {
                _currentMachinePreview.RotatePreview(_currentRotation);
            }
        }

        private void DestroyPreview()
        {
            if (_currentMachinePreview)
            {
                Destroy(_currentMachinePreview.gameObject);
            }
        }
        
        // ------------------------------------------------------------------------- GRID COMMUNICATION -------------------------------------------------------------------------------- 
        private void AddSelectedMachineToGrid()
        {
            if (!_currentMachinePreview)
            {
                return;
            }

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

            // Check if the machine can be placed on the grid. 
            foreach (var node in _currentMachinePreview.Machine.Nodes)
            {
                var nodeGridPosition = node.SetGridPosition(new Vector2Int(chosenCell.X, chosenCell.Y));

                // One node does not overlap a constructable cell. 
                if (!Grid.TryGetCellByCoordinates(nodeGridPosition.x, nodeGridPosition.y, out Cell overlapCell))
                {
                    return;
                }

                // One node of the machine overlap a cell that already contain an object. 
                if (overlapCell.ContainsObject)
                {
                    return;
                }
            }

            _gridController.AddMachineToGrid(_currentMachinePreview, chosenCell, true);

            var machineTemplate = _currentMachinePreview.Machine.Template;
            _currentMachinePreview = null;
            
            //Instantiate the same machine type if we have enough in the inventory.
            if (InventoryController.Instance.CountMachineOfType(machineTemplate) > 0)
            {
                InstantiatePreview(machineTemplate);
            }
        }
        
        // ------------------------------------------------------------------------- EVENT HANDLERS -------------------------------------------------------------------------------- 
        private void HandlePlanningFactoryState(PlanningFactoryState obj)
        {
            _isFactoryState = true;
        }
        
        private void HandleShopState(ShopState obj)
        {
            _isFactoryState = false;
        }
    }
}