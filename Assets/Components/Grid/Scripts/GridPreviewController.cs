using System;
using System.Collections.Generic;
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
        [SerializeField] private Transform _previewHolder;

        [Header("Special Rotation Behaviour")] 
        [SerializeField] private SerializableDictionary<MachineTemplate, List<RotationSubMachine>> _subMachineRotation;
        
        private UnityEngine.Camera _camera;
        
        private MachineController _currentMachinePreview;
        private MachineController _currentSubMachinePreview;
        
        private int _currentInputRotation;
        private int _currentMachineRotation;
        
        private bool _isFactoryState = true;

        
        private Grid Grid => _gridController.Grid;

        private MachineController Preview => _currentMachinePreview.gameObject.activeSelf ? _currentMachinePreview : _currentSubMachinePreview;
        
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

            if (Input.GetMouseButtonDown(1))
            {
                DestroyPreview();
            }
            if (Input.GetMouseButtonDown(0))
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
        
        private void InstantiateSubPreview(MachineTemplate template, int rotation)
        {
            DestroySubPreview();
            _currentSubMachinePreview = InstantiateMachine(template, rotation);
            _currentMachineRotation = rotation;
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
                    _previewHolder.transform.position = cell.GetCenterPosition(_gridController.OriginPosition);
                }
            }
            else
            {
                _previewHolder.transform.position = worldMousePosition;
            }

            if (!_currentMachinePreview)
            {
                return;
            }
            
            // Check for special rotation behaviour
            if (_subMachineRotation.ContainsKey(_currentMachinePreview.Machine.Template))
            {
                foreach (var rotationSubMachine in _subMachineRotation[_currentMachinePreview.Machine.Template])
                {
                    if (rotationSubMachine.TargetInputRotation != _currentInputRotation) 
                        continue;
                    
                    var potentialSubMachine = rotationSubMachine.Machine;

                    // Get the grid position
                    if (!Grid.TryGetCellByPosition(worldMousePosition, out var cell)) 
                        continue;
                    
                    // Check for potential neighbour
                    if (!_gridController.TryGetAllPotentialConnection(potentialSubMachine.Nodes, new Vector2Int(cell.X, cell.Y), out _))
                        continue;
                    
                    InstantiateSubPreview(potentialSubMachine, 0);
                    ShowPreview(false);
                            
                    return;
                }
            }
            
            ShowPreview(true);
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
            }
        }

        private void DestroyPreview()
        {
            if (_currentMachinePreview)
            {
                Destroy(_currentMachinePreview.gameObject);
            }
        }
        
        private void DestroySubPreview()
        {
            if (_currentSubMachinePreview)
            {
                Destroy(_currentSubMachinePreview.gameObject);
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
                    Debug.Log("Cannot place a machine, the cell is already occupied.");
                    return;
                }
            }
            
            var machineToAdd = InstantiateMachine(Preview.Machine.Template, _currentMachineRotation);

            _gridController.AddMachineToGrid(machineToAdd, chosenCell, true);
            
            //Instantiate the same machine type if we have enough in the inventory.
            if (InventoryController.Instance.CountMachineOfType(Preview.Machine.Template) > 0)
            {
                //InstantiatePreview(Preview.Machine.Template);
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

    [Serializable]
    public struct RotationSubMachine
    {
        public int TargetInputRotation;
        public MachineTemplate Machine;
    }
}