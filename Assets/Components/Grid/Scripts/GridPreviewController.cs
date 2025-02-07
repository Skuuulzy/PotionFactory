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
                    AutomaticPlacement();
                }
            }
            if (Input.GetMouseButtonDown(2) || Input.GetKey(KeyCode.R))
            {
                RotatePreview();
            }

            if (Input.GetMouseButtonUp(0))
            {
                _previousCells.Clear();
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
                OnPreview?.Invoke(false);
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

        private List<Cell> _previousCells = new List<Cell>();
        
        private void AutomaticPlacement()
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
            
            // Check if the cell is already the last recorded one to avoid duplicates.
            if (_previousCells.Count > 0 && _previousCells[^1] == chosenCell)
            {
                return;
            }

            // Add the new cell while keeping only the last two.
            _previousCells.Add(chosenCell);

            if (_previousCells.Count > 3)
            {
                _previousCells.RemoveAt(0);
            }

            if (_previousCells.Count != 3)
            {
                return;
            }
            
            Debug.Log("Path found !");
            Debug.Log($"Cell 0: {_previousCells[0].Position}");
            Debug.Log($"Cell 1: {_previousCells[1].Position}");
            Debug.Log($"Cell 2: {_previousCells[2].Position}");
            DetectAngle();
        }
        
        // ----------------------------------------- ANGLE DETECTION -------------------------------------------------
        private void DetectAngle()
        {
            // Get the last three cells.
            Cell cell0 = _previousCells[0];
            Cell cell1 = _previousCells[1];
            Cell cell2 = _previousCells[2];

            // Compute direction vectors.
            Vector2Int dir1 = cell1.Position - cell0.Position;
            Vector2Int dir2 = cell2.Position - cell1.Position;

            // Determine the movement type.
            string startDirection = GetDirectionName(dir1);
            string turnDirection = GetTurnDirection(dir1, dir2);
            string endDirection = GetDirectionName(dir2);

            if (startDirection != "None" && endDirection != "None" && turnDirection != "None")
            {
                Debug.Log($"[AutomaticPlacement] Player turned from {startDirection} to {turnDirection} to {endDirection}");
            }

            PlaceMachines(cell0, cell1, cell2, dir1, dir2);
        }


        // ----------------------------------------- MACHINE PLACEMENT -------------------------------------------------
        
        private void PlaceMachines(Cell cell0, Cell cell1, Cell cell2, Vector2Int dir1, Vector2Int dir2)
        {
            MachineTemplate startMachineTemplate = ScriptableObjectDatabase.GetScriptableObject<MachineTemplate>("ConveyorLeftToRight");
            MachineTemplate turnMachineTemplate;
            MachineTemplate endMachineTemplate = ScriptableObjectDatabase.GetScriptableObject<MachineTemplate>("ConveyorLeftToRight");

            int startRotation = GetRotationForDirection(dir1);
            int turnRotation = GetTurnRotation(dir1, dir2);
            int endRotation = GetRotationForDirection(dir2);

            // Determine correct turn machine type and rotation
            if (dir1.x != 0 && dir2.y != 0) // Turning from horizontal to vertical
            {
                turnMachineTemplate = ScriptableObjectDatabase.GetScriptableObject<MachineTemplate>("ConveyorLeftToDown");
            }
            else if (dir1.y != 0 && dir2.x != 0) // Turning from vertical to horizontal
            {
                turnMachineTemplate = ScriptableObjectDatabase.GetScriptableObject<MachineTemplate>("ConveyorLeftToUp");
            }
            else
            {
                Debug.LogError("[AutomaticPlacement] Invalid turn detected.");
                return;
            }

            // Instantiate machines at respective positions.
            var startMachine = InstantiateMachine(startMachineTemplate, startRotation);
            _gridController.AddMachineToGrid(startMachine, cell0, false);

            var turnMachine = InstantiateMachine(turnMachineTemplate, turnRotation);
            _gridController.AddMachineToGrid(turnMachine, cell1, false);

            var endMachine = InstantiateMachine(endMachineTemplate, endRotation);
            _gridController.AddMachineToGrid(endMachine, cell2, false);
        }

        // ----------------------------------------- HELPER METHODS -------------------------------------------------

        /// Returns a string representation of a direction based on a Vector2Int movement.
        private string GetDirectionName(Vector2Int direction)
        {
            if (direction == Vector2Int.up) return "Down to Up";
            if (direction == Vector2Int.down) return "Up to Down";
            if (direction == Vector2Int.left) return "Right to Left";
            if (direction == Vector2Int.right) return "Left to Right";
            return "None";
        }

        /// Determines the general turn direction.
        private string GetTurnDirection(Vector2Int firstDirection, Vector2Int secondDirection)
        {
            if (firstDirection == Vector2Int.right && secondDirection == Vector2Int.down) return "Left to Down";
            if (firstDirection == Vector2Int.right && secondDirection == Vector2Int.up) return "Left to Up";

            if (firstDirection == Vector2Int.left && secondDirection == Vector2Int.down) return "Right to Down";
            if (firstDirection == Vector2Int.left && secondDirection == Vector2Int.up) return "Right to Up";

            if (firstDirection == Vector2Int.up && secondDirection == Vector2Int.right) return "Down to Right";
            if (firstDirection == Vector2Int.up && secondDirection == Vector2Int.left) return "Down to Left";

            if (firstDirection == Vector2Int.down && secondDirection == Vector2Int.right) return "Up to Right";
            if (firstDirection == Vector2Int.down && secondDirection == Vector2Int.left) return "Up to Left";

            return "None";
        }

        /// Returns the appropriate rotation for a given movement direction.
        private int GetRotationForDirection(Vector2Int direction)
        {
            if (direction == Vector2Int.right) return 0; 
            if (direction == Vector2Int.down) return 90;
            if (direction == Vector2Int.left) return 180;
            if (direction == Vector2Int.up) return 270;

            return 0;
        }

        /// Returns the rotation needed for a turn piece.
        private int GetTurnRotation(Vector2Int firstDirection, Vector2Int secondDirection)
        {
            if (firstDirection == Vector2Int.right && secondDirection == Vector2Int.down) return 0;
            if (firstDirection == Vector2Int.right && secondDirection == Vector2Int.up) return 270;

            if (firstDirection == Vector2Int.left && secondDirection == Vector2Int.down) return 90;
            if (firstDirection == Vector2Int.left && secondDirection == Vector2Int.up) return 180;

            if (firstDirection == Vector2Int.up && secondDirection == Vector2Int.right) return 270;
            if (firstDirection == Vector2Int.up && secondDirection == Vector2Int.left) return 180;

            if (firstDirection == Vector2Int.down && secondDirection == Vector2Int.right) return 0;
            if (firstDirection == Vector2Int.down && secondDirection == Vector2Int.left) return 90;

            return 0;

            return 0;
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