using System;
using System.Collections.Generic;
using CodeMonkey.Utils;
using Components.Grid.Tile;
using Components.Inventory;
using Components.Machines;
using Database;
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
        [SerializeField] private Transform _previewHolder;

        [Header("Special Rotation Behaviour")]
        [SerializeField] private bool _useSubMachine;
        [SerializeField] private SerializableDictionary<MachineTemplate, List<RotationSubMachine>> _subMachineRotation;

        private Camera _camera;

        private MachineController _currentMachinePreview;
        private MachineController _currentSubMachinePreview;

        private int _currentInputRotation;
        private int _currentMachineRotation;

        private bool _isFactoryState = true;
        private Vector3 _lastCellPosition = new(-1, -1, -1);

        private bool _cleanMode;
        private bool _moveMode;

        public static Action<bool> OnPreview;

        private Grid Grid => _gridController.Grid;

        private MachineController Preview => _currentMachinePreview.gameObject.activeSelf ? _currentMachinePreview : _currentSubMachinePreview;

        // ------------------------------------------------------------------------- MONO -------------------------------------------------------------------------------- 
        private void Start()
        {
            _camera = Camera.main;

            MachineManager.OnChangeSelectedMachine += InstantiatePreview;
            PlanningFactoryState.OnPlanningFactoryStateStarted += HandlePlanningFactoryState;
            ShopState.OnShopStateStarted += HandleShopState;
            UIGrimoireController.OnEnableCleanMode += HandleCleanMode;
            GrimoireButton.OnGrimoireButtonDeselect += HandleGrimoireDeselect;
            Machine.OnMove += HandleMovingMachine;
        }

        private void Update()
        {
            //Can't interact with anything if we are not in factory state 
            if (!_isFactoryState || Grid == null)
            {
                return;
            }

            if (!_cleanMode)
            {
                MovePreview();
            }

            if (Input.GetMouseButtonDown(1) && !_moveMode)
            {
                DestroyPreview();
                OnPreview?.Invoke(false);
            }
            if (Input.GetMouseButton(0))
            {
                if (_cleanMode)
                {
                    TryDestroyHoveredMachine();
                }
                else
                {
                    AddSelectedMachineToGrid();
                }
            }
            if (Input.GetMouseButtonDown(2) || Input.GetKey(KeyCode.R))
            {
                RotatePreview();
            }
        }

        private void OnDestroy()
        {
            MachineManager.OnChangeSelectedMachine -= InstantiatePreview;
            PlanningFactoryState.OnPlanningFactoryStateStarted -= HandlePlanningFactoryState;
            ShopState.OnShopStateStarted -= HandleShopState;
            UIGrimoireController.OnEnableCleanMode -= HandleCleanMode;
            GrimoireButton.OnGrimoireButtonDeselect -= HandleGrimoireDeselect;
        }

        private MachineController InstantiateMachine(MachineTemplate template, int rotation)
        {
            var machine = Instantiate(_machineControllerPrefab, _previewHolder);
            machine.InstantiatePreview(template, Grid.GetCellSize(), true);
            machine.RotatePreview(rotation);

            return machine;
        }

        // ------------------------------------------------------------------------- PREVIEW BEHAVIOUR -------------------------------------------------------------------------------- 
        private void InstantiatePreview(MachineTemplate template)
        {
            OnPreview?.Invoke(true);
            
            DestroyPreview();
            _currentMachinePreview = InstantiateMachine(template, _currentInputRotation);
        }

        private void ShowPreview(bool show)
        {
            _currentMachinePreview.gameObject.SetActive(show);
            if (show)
            {
                _currentMachineRotation = _currentInputRotation;
            }

            if (_currentSubMachinePreview)
            {
                _currentSubMachinePreview.gameObject.SetActive(!show);
            }
        }

        private void MovePreview()
        {
            if (!UtilsClass.ScreenToWorldPositionIgnoringUI(Input.mousePosition, _camera, out Vector3 worldMousePosition))
            {
                return;
            }

            // Update the object's position 
            if (_snapping)
            {
                if (Grid.TryGetCellByPosition(worldMousePosition, out Cell cell))
                {
                    var cellPosition = cell.GetCenterPosition(_gridController.OriginPosition);

                    // Preventing the computation when staying on the same cell.
                    if (cellPosition == _lastCellPosition)
                    {
                        return;
                    }

                    _previewHolder.transform.position = cellPosition;
                    _lastCellPosition = cellPosition;

                    if (_currentMachinePreview)
                    {
                        Preview.UpdateOutlineState(IsMachinePlacable(cell));
                    }
                }
            }
            else
            {
                _previewHolder.transform.position = worldMousePosition;
            }

            // Checking for special rotational behaviors.
            if (_useSubMachine && _currentMachinePreview && _subMachineRotation.ContainsKey(_currentMachinePreview.Machine.Template))
            {
                CheckForSubPreview(worldMousePosition);
            }
        }

        private void RotatePreview()
        {
            if (!_currentMachinePreview)
            {
                return;
            }

            _currentInputRotation += 90;
            _currentInputRotation %= 360;

            _currentMachineRotation = _currentInputRotation;

            if (_currentMachinePreview != null)
            {
                _currentMachinePreview.RotatePreview(_currentInputRotation);
            }

            // Checking for special rotational behaviors.
            if (_useSubMachine && _currentMachinePreview && _subMachineRotation.ContainsKey(_currentMachinePreview.Machine.Template))
            {
                if (!UtilsClass.ScreenToWorldPositionIgnoringUI(Input.mousePosition, _camera, out Vector3 worldMousePosition))
                {
                    return;
                }

                CheckForSubPreview(worldMousePosition);
            }
        }

        private void DestroyPreview()
        {
            if (_currentMachinePreview)
            {
                Destroy(_currentMachinePreview.gameObject);
            }
        }

        // ------------------------------------------------------------------------- SUB PREVIEW BEHAVIOUR -------------------------------------------------------------------------------- 
        private void InstantiateSubPreview(MachineTemplate template, int rotation)
        {
            DestroySubPreview();
            _currentSubMachinePreview = InstantiateMachine(template, rotation);
            _currentMachineRotation = rotation;
        }

        private void DestroySubPreview()
        {
            if (_currentSubMachinePreview)
            {
                Destroy(_currentSubMachinePreview.gameObject);
            }
        }

        private void CheckForSubPreview(Vector3 worldMousePosition)
        {
            // Get the grid position
            if (!Grid.TryGetCellByPosition(worldMousePosition, out var cell))
                return;

            var leftLocalAngle = (_currentInputRotation - 90).NormalizeAngle();
            var rightLocalAngle = (_currentInputRotation + 90).NormalizeAngle();

            if (_gridController.ScanForPotentialConnection(cell.Position, leftLocalAngle.SideFromAngle(), Way.OUT))
            {
                InstantiateSubPreview(ScriptableObjectDatabase.GetScriptableObject<MachineTemplate>("ConveyorLeftToUp"), rightLocalAngle);
                ShowPreview(false);

                return;
            }

            if (_gridController.ScanForPotentialConnection(cell.Position, rightLocalAngle.SideFromAngle(), Way.OUT))
            {
                InstantiateSubPreview(ScriptableObjectDatabase.GetScriptableObject<MachineTemplate>("ConveyorLeftToDown"), leftLocalAngle);
                ShowPreview(false);

                return;
            }

            ShowPreview(true);
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
            if (!IsMachinePlacable(chosenCell))
            {
                return;
            }

            if (_moveMode)
            {
                _gridController.AddMachineToGrid(_currentMachinePreview, chosenCell, false);
                _currentMachinePreview = null;
                _moveMode = false;
            }
            else
            {
                var machineToAdd = InstantiateMachine(Preview.Machine.Template, _currentMachineRotation);
                _gridController.AddMachineToGrid(machineToAdd, chosenCell, true);
                
                //Instantiate the same machine type if we have enough in the inventory.
                if (GrimoireController.Instance.CountMachineOfType(Preview.Machine.Template.Type) <= 0)
                {
                    DestroyPreview();
                }
            }
        }

        private void TryDestroyHoveredMachine()
        {
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

            if (!chosenCell.ContainsNode)
            {
                return;
            }

            chosenCell.Node.Machine.Controller.Retrieve();
        }

        private bool IsMachinePlacable(Cell originCell)
        {
            foreach (var node in Preview.Machine.Nodes)
            {
                var nodeGridPosition = node.SetGridPosition(new Vector2Int(originCell.X, originCell.Y));

                // One node does not overlap a constructable cell. 
                if (!Grid.TryGetCellByCoordinates(nodeGridPosition.x, nodeGridPosition.y, out Cell overlapCell))
                {
                    return false;
                }
                
                if (!overlapCell.IsConstructable())
                {
                    return false;
                }
            }

            return true;
        }
        
        // ------------------------------------------------------------------------- EVENT HANDLERS -------------------------------------------------------------------------------- 
        private void HandlePlanningFactoryState(PlanningFactoryState _)
        {
            _isFactoryState = true;
        }

        private void HandleShopState(ShopState _)
        {
            _isFactoryState = false;
        }

        private void HandleCleanMode(bool cleanMode)
        {
            _cleanMode = cleanMode;

            // Hide the preview
            if (_currentMachinePreview)
            {
                _currentMachinePreview.gameObject.SetActive(!cleanMode);
            }
        }

        private void HandleGrimoireDeselect()
        {
            DestroyPreview();
        }
        
        // For now to move a machine we first destroy it and the reinstancing it.
        private void HandleMovingMachine(Machine machine)
        {
            _currentMachinePreview = machine.Controller;
            _currentMachineRotation = (int)machine.Controller.transform.rotation.eulerAngles.y;
            
            machine.Controller.transform.parent = _previewHolder;
            machine.Controller.transform.localPosition = Vector3.zero;

            _moveMode = true;
        }
    }

    [Serializable]
    public struct RotationSubMachine
    {
        public int TargetInputRotation;
        public MachineTemplate Machine;
    }
}