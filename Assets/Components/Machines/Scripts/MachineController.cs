using System;
using System.Collections.Generic;
using Components.Grid;
using UnityEngine;

namespace Components.Machines
{
    public class MachineController : GridObjectController
    {
        [Header("Arrow Preview")]
        [SerializeField] private GameObject _inPreview;
        [SerializeField] private GameObject _outPreview;
        
        [Header("Outline")]
        [SerializeField] private Color _placableColor = Color.green;
        [SerializeField] private Color _unPlacableColor = Color.red;
        [SerializeField] private Color _selectedColor = Color.blue;
        [SerializeField] private Color _hoveredColor = Color.white;

        [Header("DEBUG")]
        [SerializeField] private Machine _machine;

        [Header("Animator")]
        [SerializeField] private Animator _animator;

        private const string PLAY_MACHINE_ANIM = "MachineProcessing";

        private Outline _outline;
        private List<GameObject> _directionalArrows;
        
        private readonly List<MachineGridComponent> _gridComponents = new();
        
        private bool _initialized;
        private bool _selected;
        
        public Machine Machine => _machine;
        
        // ------------------------------------------------------------------------- INIT -----------------------------------------------------------------------------
        protected override void InstantiateView(GridObjectTemplate template, Quaternion localRotation, Vector3 localScale)
        {
            base.InstantiateView(template, localRotation, localScale);
            
            _animator = View.GetComponentInChildren<Animator>();
            if (Template is MachineTemplate machineTemplate)
            {
                _machine = new Machine(machineTemplate, this);
                SetupDirectionalArrows(machineTemplate);
            }
            else
            {
                Debug.LogError("Please give a machine template to instantiate a machine preview.");
            }
            
            _outline = View.AddComponent<Outline>();
            _outline.OutlineWidth = 8;
            
            // Instantiate grid components
            for (int i = 0; i < _machine.Template.GridComponents.Count; i++)
            {
                var gridComponent = Instantiate(_machine.Template.GridComponents[i], transform);
                gridComponent.Initialize(_machine);
                _gridComponents.Add(gridComponent);
            }

            Machine.OnProcess += HandleProcessMachine;
        }

		private void HandleProcessMachine(Machine machine, bool value)
		{
            if(machine == Machine && _animator != null)
			{
                _animator.SetBool(PLAY_MACHINE_ANIM, value);
            }
        }

		public void RotatePreview(int angle)
        {
            View.transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
            _machine.UpdateNodesRotation(angle);

            for (int i = 0; i < _gridComponents.Count; i++)
            {
                _gridComponents[i].transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
            }
        }
        
        public void ConfirmPlacement()
        {
            Machine.OnSelected += HandleMachineSelected;
            Machine.OnHovered += HandleMachineHovered;
            
            _initialized = true;
            _machine.LinkNodeData();

            _machine.AddMachineToChain();
            
            ToggleDirectionalArrows(false);
            ToggleOutlines(false);
        }

        public override void AddToGrid(Cell originCell, Grid.Grid grid, Transform holder)
        {
            base.AddToGrid(originCell, grid, holder);
            
            // Adding nodes to the cells.
            foreach (var node in Machine.Nodes)
            {
                var nodeGridPosition = node.SetGridPosition(originCell.Coordinates);

                if (grid.TryGetCellByCoordinates(nodeGridPosition.x, nodeGridPosition.y, out Cell overlapCell))
                {
                    overlapCell.AddNodeToCell(node);
					
                    // Add potential connected ports 
                    foreach (var port in node.Ports)
                    {
                        switch (port.Side)
                        {
                            case Side.DOWN:
                                port.TryConnectPort(new Vector2Int(nodeGridPosition.x, nodeGridPosition.y - 1), grid);
                                break;
                            case Side.UP:
                                port.TryConnectPort(new Vector2Int(nodeGridPosition.x, nodeGridPosition.y + 1), grid);
                                break;
                            case Side.RIGHT:
                                port.TryConnectPort(new Vector2Int(nodeGridPosition.x + 1, nodeGridPosition.y), grid);
                                break;
                            case Side.LEFT:
                                port.TryConnectPort(new Vector2Int(nodeGridPosition.x - 1, nodeGridPosition.y), grid);
                                break;
                            case Side.NONE:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
			
            ConfirmPlacement();
        }

        // ------------------------------------------------------------------------- DESTROY --------------------------------------------------------------------------
        private void OnDestroy()
        {
            Machine.OnSelected -= HandleMachineSelected;
            Machine.OnHovered -= HandleMachineHovered;
            
            if (!_initialized)
            {
                return;
            }
            
            _machine.RemoveMachineFromChain();
        }

        // ------------------------------------------------------------------------- CONTEXTUAL ACTIONS ---------------------------------------------------------------
        public void Move()
        {
            Machine.OnMove?.Invoke(Machine);
        }

        public void Configure()
        {
            Machine.OnConfigure?.Invoke(Machine);
        }

        public void Retrieve()
        {
            Machine.OnRetrieve?.Invoke(Machine, true);
        }
        
        // ------------------------------------------------------------------------- HANDLERS -------------------------------------------------------------------------
        private void HandleMachineSelected(Machine machine, bool selected)
        {
            if (Machine != machine)
            {
                ToggleDirectionalArrows(false);
                ToggleOutlines(false);
                _selected = false;
                
                return;
            }

            _selected = true;
            ToggleDirectionalArrows(selected);
            ToggleOutlines(selected, _selectedColor);
        }
        
        private void HandleMachineHovered(Machine machine, bool hovered)
        {
            if (Machine != machine)
            {
                ToggleOutlines(false);
                ToggleDirectionalArrows(false);
                return;
            }
            
            ToggleDirectionalArrows(hovered);
            ToggleOutlines(hovered, _hoveredColor);
        }
        
        // ------------------------------------------------------------------------- DIRECTIONAL ARROWS ---------------------------------------------------------------
        private void SetupDirectionalArrows(MachineTemplate machineTemplate)
        {
            _directionalArrows = new List<GameObject>();
            
            foreach (var node in machineTemplate.Nodes)
            {
                foreach (var port in node.Ports)
                {
                    var previewArrow = Instantiate(port.Way == Way.IN ? _inPreview : _outPreview, View.transform);
                    
                    previewArrow.transform.localPosition = new Vector3(node.LocalPosition.x, previewArrow.transform.position.y, node.LocalPosition.y);
                    
                    // TODO: Find why we need to invert the angle when the machine is only 1x1 (especially with curved conveyor).
                    previewArrow.transform.Rotate(Vector3.up, port.Side.AngleFromSide(machineTemplate.Nodes.Count == 1));
                    
                    _directionalArrows.Add(previewArrow);
                }
            }
        }
        
        private void ToggleDirectionalArrows(bool toggle)
        {
            if (!this)
            {
                return;
            }
            
            for (int i = 0; i < _directionalArrows.Count; i++)
            {
                _directionalArrows[i].SetActive(toggle);
            }
        }
        
        // ------------------------------------------------------------------------- OUTLINES -------------------------------------------------------------------------
        private void ToggleOutlines(bool toggle, Color outlineColor = default)
        {
            if (!_outline)
            {
                return;
            }
            
            _outline.enabled = toggle;
            _outline.OutlineColor = outlineColor;
        }
        
        public void UpdateOutlineState(bool placable)
        {
            if (!_outline)
            {
                return;
            }
            
            _outline.OutlineColor = placable ? _placableColor : _unPlacableColor;
        }
    }
}