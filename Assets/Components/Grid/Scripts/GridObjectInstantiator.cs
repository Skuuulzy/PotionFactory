using System;
using System.Collections.Generic;
using CodeMonkey.Utils;
using Components.Inventory;
using Components.Machines;
using Database;
using UnityEngine;

namespace Components.Grid
{
    /// Allow player to add <see cref="GridObjectController"/> to the grid.
    public class GridObjectInstantiator : MonoBehaviour
    {
        [Header("Movement Parameters")]
        [SerializeField] private bool _snapping;

        [Header("Components")]
        [SerializeField] private GridController _gridController;
        [SerializeField] private MachineController _machineControllerPrefab;
        [SerializeField] private Transform _previewHolder;
        [SerializeField] private Transform _gridObjectsHolder;

        [Header("Special Rotation Behaviour")]
        [SerializeField] private SerializableDictionary<MachineTemplate, List<RotationSubMachine>> _subMachineRotation;

        [Header("Debug")]
        [SerializeField] private InputState _inputState = InputState.SELECTION;
        [SerializeField] private MachineController _currentMachinePreview;
        [SerializeField] private int _currentInputRotation;
        [SerializeField] private int _currentPreviewRotation;
        [SerializeField] private bool _moveMode;
        [SerializeField] private bool _justPlaced;
        [SerializeField] private bool _justRemoved;
        
        private Camera _camera;

        private bool _isFactoryState = true;
        private Vector3 _lastCellPosition = new(-1, -1, -1);
        private Machine _hoveredMachine;
        
        private enum InputState
        {
            SELECTION,
            PLACEMENT
        }

        private Grid Grid => _gridController.Grid;
        
        public static Action<bool> OnPreview;
        
        // ------------------------------------------------------------------------- MONO -------------------------------------------------------------------------------- 
        private void Start()
        {
            _camera = Camera.main;
            _inputState = InputState.SELECTION;

            MachineManager.OnChangeSelectedMachine += InstantiatePreview;
            GrimoireButton.OnGrimoireButtonDeselect += HandleGrimoireDeselect;
            
            Machine.OnMove += HandleMovingMachine;
            
            ResolutionFactoryState.OnResolutionFactoryStateStarted += HandleResolutionFactoryState;
            EndOfDayState.OnEndOfDayStateStarted += HandleShopState;
        }
        
        private void OnDestroy()
        {
            MachineManager.OnChangeSelectedMachine -= InstantiatePreview;
            GrimoireButton.OnGrimoireButtonDeselect -= HandleGrimoireDeselect;
            
            Machine.OnMove -= HandleMovingMachine;
            
            ResolutionFactoryState.OnResolutionFactoryStateStarted -= HandleResolutionFactoryState;
            EndOfDayState.OnEndOfDayStateStarted -= HandleShopState;
        }

        private void Update()
        {
            //Can't interact with anything if we are not in factory state 
            if (!_isFactoryState || Grid == null)
            {
                return;
            }

            switch (_inputState)
            {
                case InputState.SELECTION:
                    HandleSelectionMode();
                    break;
            
                case InputState.PLACEMENT:
                    HandlePlacementMode();
                    break;
            }
        }
        
        private void HandleSelectionMode()
        {
            TryHoverMachine();   

            if (Input.GetMouseButtonUp(0))
            {
                TryMoveMachine();
            }
            if (Input.GetMouseButton(1))
            {
                TryRetrieveMachine();
            }
        }
        
        private void HandlePlacementMode()
        {
            MovePreview();
    
            if (Input.GetMouseButton(0))
            {
                AddSelectedMachineToGrid();
            }
            if (Input.GetMouseButtonDown(1))
            {
                DestroyPreview();
                SwitchInputState(InputState.SELECTION);
                ResetFlags();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                RotatePreview();
            }
        }

        private void SwitchInputState(InputState newState)
        {
            _inputState = newState;
        }

        private void ResetFlags()
        {
            // Reset flags when switching input states
            _currentInputRotation = 0;
            _currentPreviewRotation = 0;
            _justPlaced = false;
            _justRemoved = false;
        }

        // ------------------------------------------------------------------------- PREVIEW BEHAVIOUR -------------------------------------------------------------------------------- 

        private void InstantiatePreview(MachineTemplate template)
        {
            InstantiatePreview(template, 0);
        }
        
        private void InstantiatePreview(MachineTemplate template, int rotation)
        {
            // Prevent instantiation of the same type.
            if (_currentMachinePreview && _currentMachinePreview.Machine.Template == template)
            {
                return;
            }
            
            OnPreview?.Invoke(false);
            
            DestroyPreview();
            _currentMachinePreview = InstantiateMachine(template, rotation);
            _currentPreviewRotation = rotation;
            
            SwitchInputState(InputState.PLACEMENT);
            
            Debug.Log($"Selecting {_currentMachinePreview.name}");
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
                        _currentMachinePreview.UpdateOutlineState(IsMachinePlacable(cell));
                    }
                }
            }
            else
            {
                _previewHolder.transform.position = worldMousePosition;
            }
            
            CheckForSubPreview(worldMousePosition);
        }

        private void RotatePreview()
        {
            if (!_currentMachinePreview)
            {
                return;
            }

            _currentInputRotation += 90;
            _currentInputRotation %= 360;
            
            if (_currentMachinePreview != null)
            {
                _currentMachinePreview.RotatePreview(_currentInputRotation);
                _currentPreviewRotation = _currentInputRotation;
            }

            if (!UtilsClass.ScreenToWorldPositionIgnoringUI(Input.mousePosition, _camera, out Vector3 worldMousePosition))
            {
                return;
            }

            CheckForSubPreview(worldMousePosition);
        }

        private void DestroyPreview()
        {
            if (_currentMachinePreview)
            {
                if (!_currentMachinePreview.Machine.Template.CanRetrieve)
                {
                    return;
                }

                Debug.Log("Destroying preview");
                
                Destroy(_currentMachinePreview.gameObject);
                _justRemoved = true;
                
                OnPreview?.Invoke(false);
            }
        }

        // ------------------------------------------------------------------------- SUB PREVIEW BEHAVIOUR -------------------------------------------------------------------------------- 

        private void CheckForSubPreview(Vector3 worldMousePosition)
        {
            if (_currentMachinePreview.Machine.Template.Type != MachineType.CONVEYOR)
            {
                return;
            }
            
            // Get the grid position
            if (!Grid.TryGetCellByPosition(worldMousePosition, out var cell))
                return;

            var leftLocalAngle = (_currentInputRotation - 90).NormalizeAngle();
            var rightLocalAngle = (_currentInputRotation + 90).NormalizeAngle();

            if (ScanForPotentialConnection(cell.Coordinates, leftLocalAngle.SideFromAngle(), Way.OUT))
            {
                InstantiatePreview(ScriptableObjectDatabase.GetScriptableObject<MachineTemplate>("ConveyorLeftToUp"), rightLocalAngle);
                
                return;
            }

            if (ScanForPotentialConnection(cell.Coordinates, rightLocalAngle.SideFromAngle(), Way.OUT))
            {
                InstantiatePreview(ScriptableObjectDatabase.GetScriptableObject<MachineTemplate>("ConveyorLeftToDown"), leftLocalAngle);

                return;
            }

            InstantiatePreview(ScriptableObjectDatabase.GetScriptableObject<MachineTemplate>("ConveyorLeftToRight"), _currentInputRotation);
        }
        
        private bool ScanForPotentialConnection(Vector2Int cellPosition, Side sideToScan, Way desiredWay)
        {
            var neighbourPosition = sideToScan.GetNeighbourPosition(cellPosition);
			
            if (Grid.TryGetCellByCoordinates(neighbourPosition.x, neighbourPosition.y, out var cell))
            {
                if (cell.ContainsNode)
                {
                    foreach (var port in cell.Node.Ports)
                    {
                        if (port.Side == sideToScan.Opposite() && port.Way == desiredWay)
                        {
                            Debug.Log($"Found a potential connection on cell: {cell.Coordinates}");
                            return true;
                        }
                    }
                }
            }

            return false;
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
                Debug.Log($"Adding machine :{_currentMachinePreview.Machine.Controller.name} to grid from a movement.");
                
                _currentMachinePreview.AddToGrid(chosenCell, Grid, _gridObjectsHolder);
                _currentMachinePreview.Machine.Select(false);
                _currentMachinePreview = null;
                _moveMode = false;
                _justPlaced = true;
                SwitchInputState(InputState.SELECTION);
            }
            else
            {
                Debug.Log($"Adding machine :{_currentMachinePreview.Machine.Controller.name} from inventory to grid");
                
                var machineToAdd = InstantiateMachine(_currentMachinePreview.Machine.Template, _currentPreviewRotation);
                machineToAdd.AddToGrid(chosenCell, Grid, _gridObjectsHolder);
                GrimoireController.Instance.DecreaseMachineToPlayerInventory(machineToAdd.Machine.Template, 1);
                
                _justPlaced = true;
                
                //Instantiate the same machine type if we have enough in the inventory.
                if (GrimoireController.Instance.CountMachineOfType(_currentMachinePreview.Machine.Template.Type) <= 0)
                {
                    DestroyPreview();
                }
            }
        }

        private bool IsMachinePlacable(Cell originCell)
        {
            foreach (var node in _currentMachinePreview.Machine.Nodes)
            {
                var nodeGridPosition = node.SetGridPosition(originCell.Coordinates);

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
        
        // ------------------------------------------------------------------------- MANIPULATE MACHINES -------------------------------------------------------------------------------- 
        
        private void TryMoveMachine()
        {
            // Prevent placing a machine and then directly move it.
            if (_justPlaced)
            {
                _justPlaced = false;
                return;
            }
            
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
                var machine = chosenCell.Node.Machine;

                if (!machine.Template.CanMove)
                {
                    return;
                }
                
                Debug.Log($"Move machine {machine.Controller.name}");
                machine.Controller.Move();
                machine.Select(true);
                SwitchInputState(InputState.PLACEMENT);
            }
        }
        
        private void TryRetrieveMachine()
        {
            if (_justRemoved)
            {
                _justRemoved = false;
                return;
            }
            
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
                return;

            if (!chosenCell.Node.Machine.Template.CanRetrieve)
            {
                return;
            }
            
            Debug.Log($"Retrieve machine { chosenCell.Node.Machine.Controller.name}");
            chosenCell.Node.Machine.Controller.Retrieve();
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
        
        // ------------------------------------------------------------------------- EVENT HANDLERS -------------------------------------------------------------------------------- 
        private void HandleResolutionFactoryState(ResolutionFactoryState _)
        {
            _isFactoryState = true;
        }

        private void HandleShopState(EndOfDayState _)
        {
            _isFactoryState = false;
        }

        private void HandleGrimoireDeselect()
        {
            DestroyPreview();
        }
        
        private void HandleMovingMachine(Machine machine)
        {
            _currentMachinePreview = machine.Controller;
            _currentInputRotation = (int)machine.Controller.transform.rotation.eulerAngles.y;
            
            machine.Controller.transform.parent = _previewHolder;
            machine.Controller.transform.localPosition = Vector3.zero;

            _moveMode = true;
        }
        
        // ------------------------------------------------------------------------- HELPERS -------------------------------------------------------------------------------- 

        private MachineController InstantiateMachine(MachineTemplate template, int rotation)
        {
            var gridObjectController = GridObjectController.InstantiateFromTemplate(template, Grid.GetCellSize(), _previewHolder);
            if (gridObjectController is MachineController machineController)
            {
                machineController.RotatePreview(rotation);

                return machineController;
            }
            
            Debug.LogError("The preview instantiated is not of type MachineController.");
            return null;
        }
    }

    [Serializable]
    public struct RotationSubMachine
    {
        public int TargetInputRotation;
        public MachineTemplate Machine;
    }
}