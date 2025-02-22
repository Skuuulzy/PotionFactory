using System;
using System.Collections.Generic;
using CodeMonkey.Utils;
using Components.Inventory;
using Components.Machines;
using Components.Tick;
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
        [SerializeField] private bool _enabled = true;
        [SerializeField] private InputState _inputState = InputState.SELECTION;
        [SerializeField] private MachineController _currentMachinePreview;
        [SerializeField] private int _currentInputRotation;
        [SerializeField] private int _currentPreviewRotation;
        [SerializeField] private bool _moveMode;
        [SerializeField] private bool _skipFrame;
        
        // ------------------------------------------------------------------------- PRIVATE FIELDS -------------------------------------------------------------------------------- 
        
        private Camera _camera;

        private Vector3 _lastCellPosition = new(-1, -1, -1);
        private Machine _hoveredMachine;
        
        private Grid Grid => _gridController.Grid;
        
        private enum InputState
        {
            SELECTION,
            PLACEMENT
        }

        // ------------------------------------------------------------------------- PUBLIC FIELDS -------------------------------------------------------------------------------- 
        
        public static Action<bool> OnPreview;
        
        // ------------------------------------------------------------------------- MONO -------------------------------------------------------------------------------- 
        private void Start()
        {
            _camera = Camera.main;
            _inputState = InputState.SELECTION;

            MachineManager.OnChangeSelectedMachine += InstantiatePreview;
            
            ResolutionFactoryState.OnResolutionFactoryStateStarted += HandleResolutionFactoryState;
            EndOfDayState.OnEndOfDayStateStarted += HandleShopState;
        }
        
        private void OnDestroy()
        {
            MachineManager.OnChangeSelectedMachine -= InstantiatePreview;
            
            ResolutionFactoryState.OnResolutionFactoryStateStarted -= HandleResolutionFactoryState;
            EndOfDayState.OnEndOfDayStateStarted -= HandleShopState;
        }

        private void Update()
        {
            //Can't interact with anything if we are not in factory state 
            if (!_enabled || Grid == null)
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
        
        // ------------------------------------------------------------------------- INPUT STATES -------------------------------------------------------------------------------- 
        
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
                PlacePreview();
            }
            if (Input.GetMouseButtonUp(1) || Input.GetKeyUp(KeyCode.Escape))
            {
                // If we can destroy the preview we go to selection mode.
                if (TryDestroyPreview())
                {
                    SwitchInputState(InputState.SELECTION);
                
                    ResetFlags();
                    SetPlacementRotations(0);
                }
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
            _skipFrame = false;
        }

        private void SetPlacementRotations(int value)
        {
            _currentInputRotation = value;
            _currentPreviewRotation = value;
        }

        // ------------------------------------------------------------------------- PLACEMENT BEHAVIOUR -------------------------------------------------------------------------------- 
        
        private void PlacePreview()
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
                if (!_currentMachinePreview.Machine.CanOverwrite(chosenCell))
                {
                    return;
                }

                // Retrieve the machine under it.
                RetrieveMachine(chosenCell.Node.Machine, true);
            }
            
            // Place the current preview on the grid.
            Debug.Log($"Adding machine: {_currentMachinePreview.name} to grid.");
            
            _currentMachinePreview.AddToGrid(chosenCell, Grid, _gridObjectsHolder);
            _currentMachinePreview.Machine.Select(false);
            _currentMachinePreview.ToggleBlueprintMaterials(false);

            // If this is not a move mode take a machine from player inventory.
            if (!_moveMode)
            {
                GrimoireController.Instance.DecreaseMachineToPlayerInventory(_currentMachinePreview.Machine.Template, 1);
                Debug.Log($"Taking {_currentMachinePreview.Machine.Template.Type} from player inventory.");
            }
            else
            {
                _moveMode = false;
            }
            
            // If there is no machine of this type in the inventory go back in selection mode.
            if (GrimoireController.Instance.CountMachineOfType(_currentMachinePreview.Machine.Template.Type) <= 0)
            {
                Debug.Log($"No more machine of type {_currentMachinePreview.Machine.Template.Type} in player inventory.");
                _currentMachinePreview = null;
                _skipFrame = true;
                SwitchInputState(InputState.SELECTION);
                return;
            }

            //Instantiate the same machine type if we have enough in the inventory.
            InstantiateNewPreviewFrom(_currentMachinePreview, _currentPreviewRotation);
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
                        _currentMachinePreview.UpdateOutlineState(IsMachinePlacable(cell) || _currentMachinePreview.Machine.CanOverwrite(cell));
                        _currentMachinePreview.ToggleBlueprintMaterials(true);
                    }
                }
            }
            else
            {
                _previewHolder.transform.position = worldMousePosition;
            }
            
            CheckForAutoConveyorOrientation(worldMousePosition);
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

            CheckForAutoConveyorOrientation(worldMousePosition);
        }

        private bool TryDestroyPreview()
        {
            if (_currentMachinePreview)
            {
                if (!_currentMachinePreview.Machine.Template.CanRetrieve)
                {
                    return false;
                }
                Debug.Log($"Destroying current preview: {_currentMachinePreview.Machine.Template.Type}");
                
                Destroy(_currentMachinePreview.gameObject);
                _currentMachinePreview = null;
                _skipFrame = true;
                
                OnPreview?.Invoke(false);
                
            }

            return true;
        }
        
        // ------------------------------------------------------------------------- SELECTION BEHAVIOUR -------------------------------------------------------------------------------- 
        
        private void TryMoveMachine()
        {
            // Prevent placing a machine and then directly move it.
            if (_skipFrame)
            {
                _skipFrame = false;
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
                
                Debug.Log($"Moving machine {machine.Controller.name}.");
                
                ClearMachineGridData(machine);
            
                _currentMachinePreview = machine.Controller;
                _currentInputRotation = machine.Rotation;
            
                machine.Controller.transform.parent = _previewHolder;
                machine.Controller.transform.localPosition = Vector3.zero;
                machine.Select(true);

                _moveMode = true;
                SetPlacementRotations(machine.Rotation);
                
                SwitchInputState(InputState.PLACEMENT);
            }
        }
        
        private void TryRetrieveMachine()
        {
            if (_skipFrame)
            {
                _skipFrame = false;
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
            RetrieveMachine(chosenCell.Node.Machine, true);
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
            _enabled = true;
        }

        private void HandleShopState(EndOfDayState _)
        {
            _enabled = false;
        }
        
        // ------------------------------------------------------------------------- HELPERS -------------------------------------------------------------------------------- 

        private void InstantiatePreview(MachineTemplate template)
        {
            SetPlacementRotations(0);          
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
            
            TryDestroyPreview();
            _currentMachinePreview = (MachineController)InstantiateGridObject(template, rotation);
            _currentPreviewRotation = rotation;
            
            SwitchInputState(InputState.PLACEMENT);
        }

        private void InstantiateNewPreviewFrom(MachineController preview, int rotation)
        {
            _currentMachinePreview = (MachineController)InstantiateGridObject(preview.Machine.Template, rotation);
            _currentPreviewRotation = rotation;
        }
        
        private GridObjectController InstantiateGridObject(GridObjectTemplate template, int rotation)
        {
            var gridObjectController = GridObjectController.InstantiateFromTemplate(template, Grid.GetCellSize(), _previewHolder);
            gridObjectController.transform.name = $"{template.Name}_{_gridObjectsHolder.childCount}";

            Debug.Log($"Instantiate grid object: {gridObjectController.transform.name}.");
            
            if (gridObjectController is MachineController machineController)
            {
                machineController.RotatePreview(rotation);
                machineController.ToggleBlueprintMaterials(true);

                return machineController;
            }
            
            Debug.LogError("The preview instantiated is not of type MachineController.");
            return null;
        }
        
        private void CheckForAutoConveyorOrientation(Vector3 worldMousePosition)
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
                Debug.Log($"Found good connection for curved conveyor: LeftToUp at {rightLocalAngle}°");
                InstantiatePreview(ScriptableObjectDatabase.GetScriptableObject<MachineTemplate>("ConveyorLeftToUp"), rightLocalAngle);
                
                return;
            }

            if (ScanForPotentialConnection(cell.Coordinates, rightLocalAngle.SideFromAngle(), Way.OUT))
            {
                Debug.Log($"Found good connection for curved conveyor: LeftToDown at {leftLocalAngle}°");
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
                            return true;
                        }
                    }
                }
            }

            return false;
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
        
        private void ClearMachineGridData(Machine machineToClear)
        {
            //Reset all cell linked to the machine.
            foreach (var node in machineToClear.Nodes)
            {
                if (!Grid.TryGetCellByCoordinates(node.GridPosition.x, node.GridPosition.y, out Cell linkedCell))
                {
                    continue;
                }

                linkedCell.RemoveNodeFromCell();
            }
            
            machineToClear.RemoveMachineFromChain();
        }

        private void RetrieveMachine(Machine machineToSell, bool giveBack)
        {
            Debug.Log($"Retrieving machine: {machineToSell.Controller.name}");
            
            ClearMachineGridData(machineToSell);
			
            // Remove 3D objects
            Destroy(machineToSell.Controller.gameObject);

            // Give back to the player
            if (giveBack)
            {
                GrimoireController.Instance.AddMachineToPlayerInventory(machineToSell.Template, 1);
            }
			
            // For destroying the class instance, not sure if this a good way.
            machineToSell = null;
        }
    }

    [Serializable]
    public struct RotationSubMachine
    {
        public int TargetInputRotation;
        public MachineTemplate Machine;
    }
}