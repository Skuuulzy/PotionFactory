using System;
using System.Collections.Generic;
using CodeMonkey.Utils;
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

        private UnityEngine.Camera _camera;

        private MachineController _currentMachinePreview;
        private MachineController _currentSubMachinePreview;

        private int _currentInputRotation;
        private int _currentMachineRotation;

        private bool _isFactoryState = true;
        private Vector3 _lastCellPosition = new(-1, -1, -1);

        private bool _cleanMode;

        public static Action<bool> OnPreviewUnselected; //True if their is a preview, false if not.

        private Grid Grid => _gridController.Grid;

        private MachineController Preview => _currentMachinePreview.gameObject.activeSelf ? _currentMachinePreview : _currentSubMachinePreview;

        // ------------------------------------------------------------------------- MONO -------------------------------------------------------------------------------- 
        private void Start()
        {
            _camera = UnityEngine.Camera.main;

            MachineManager.OnChangeSelectedMachine += InstantiatePreview;
            PlanningFactoryState.OnPlanningFactoryStateStarted += HandlePlanningFactoryState;
            ShopState.OnShopStateStarted += HandleShopState;
            UIGrimoireController.OnEnableCleanMode += HandleCleanMode;
            GrimoireButton.OnGrimoireButtonDeselect += HandleGrimoireDeselect;
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

            if (Input.GetMouseButtonDown(1))
            {
                DestroyPreview();
                OnPreviewUnselected?.Invoke(_currentMachinePreview);
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
            UIGrimoireController.OnEnableCleanMode -= HandleCleanMode;
            GrimoireButton.OnGrimoireButtonDeselect -= HandleGrimoireDeselect;
        }

        private MachineController InstantiateMachine(MachineTemplate template, int rotation)
        {
            var machine = Instantiate(_machineControllerPrefab, _previewHolder);
            machine.InstantiatePreview(template, Grid.GetCellSize());
            machine.RotatePreview(rotation);

            return machine;
        }

        // ------------------------------------------------------------------------- PREVIEW BEHAVIOUR -------------------------------------------------------------------------------- 
        private void InstantiatePreview(MachineTemplate template)
        {
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
            foreach (var node in Preview.Machine.Nodes)
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

            var machineToAdd = InstantiateMachine(Preview.Machine.Template, _currentMachineRotation);

            _gridController.AddMachineToGrid(machineToAdd, chosenCell, true);

            //Instantiate the same machine type if we have enough in the inventory.
            if (GrimoireController.Instance.CountMachineOfType(Preview.Machine.Template.Type) <= 0)
            {
				DestroyPreview();

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

            _gridController.SellMachine(chosenCell.Node.Machine, 0);
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
    }

    [Serializable]
    public struct RotationSubMachine
    {
        public int TargetInputRotation;
        public MachineTemplate Machine;
    }
}