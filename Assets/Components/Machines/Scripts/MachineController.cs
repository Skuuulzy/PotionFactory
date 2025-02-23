using System;
using System.Collections.Generic;
using Components.Grid;
using UnityEngine;

namespace Components.Machines
{
    public class MachineController : GridObjectController
    {
        private static readonly int MACHINE_PROCESSING = Animator.StringToHash(PLAY_MACHINE_ANIM);

        [Header("Arrow Preview")]
        [SerializeField] private GameObject _inPreview;
        [SerializeField] private GameObject _outPreview;
        
        [Header("Animator")]
        [SerializeField] private Animator _animator;

        [Header("DEBUG")]
        [SerializeField] private Machine _machine;
        
        private const string PLAY_MACHINE_ANIM = "MachineProcessing";

        private List<GameObject> _directionalArrows;
        private readonly List<MachineGridComponent> _gridComponents = new();
        
        public Machine Machine => _machine;
        
        // ------------------------------------------------------------------------- INIT -----------------------------------------------------------------------------
        protected override void InstantiateView(GridObjectTemplate template, Quaternion localRotation, Vector3 localScale)
        {
            base.InstantiateView(template, localRotation, localScale);
            
            _animator = GridObjectView.GetComponentInChildren<Animator>();
            
            if (Template is MachineTemplate machineTemplate)
            {
                _machine = new Machine(machineTemplate, this);
                SetupDirectionalArrows(machineTemplate);
            }
            else
            {
                Debug.LogError("Please give a machine template to instantiate a machine preview.");
            }
            
            GridObjectView.ToggleBlueprintMaterials(true);
            
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
            if(machine == Machine && _animator)
			{
                _animator.SetBool(MACHINE_PROCESSING, value);
            }
        }

		public void RotatePreview(int angle)
        {
            GridObjectView.transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
            _machine.UpdateNodesRotation(angle);

            for (int i = 0; i < _gridComponents.Count; i++)
            {
                _gridComponents[i].transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
            }
        }

        private void ConfirmPlacement()
        {
            Machine.OnSelected += HandleMachineSelected;
            Machine.OnHovered += HandleMachineHovered;
            
            _machine.LinkNodeData();
            _machine.AddMachineToChain();
            
            ToggleDirectionalArrows(false);
            GridObjectView.ToggleBlueprintMaterials(false);
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
        }

        // ------------------------------------------------------------------------- CONTEXTUAL ACTIONS ---------------------------------------------------------------

        public void Configure()
        {
            Machine.OnConfigure?.Invoke(Machine);
        }
        
        // ------------------------------------------------------------------------- HANDLERS -------------------------------------------------------------------------
        private void HandleMachineSelected(Machine machine, bool selected)
        {
            if (Machine != machine)
            {
                ToggleDirectionalArrows(false);
                GridObjectView.ToggleBlueprintMaterials(false);
                return;
            }

            ToggleDirectionalArrows(selected);
            GridObjectView.ToggleBlueprintMaterials(selected);
        }
        
        private void HandleMachineHovered(Machine machine, bool hovered)
        {
            if (Machine != machine)
            {
                GridObjectView.ToggleHoverOutlines(false);
                ToggleDirectionalArrows(false);
                return;
            }
            
            ToggleDirectionalArrows(hovered);
            GridObjectView.ToggleHoverOutlines(hovered);
        }
        
        // ------------------------------------------------------------------------- DIRECTIONAL ARROWS ---------------------------------------------------------------
        private void SetupDirectionalArrows(MachineTemplate machineTemplate)
        {
            _directionalArrows = new List<GameObject>();
            
            foreach (var node in machineTemplate.Nodes)
            {
                foreach (var port in node.Ports)
                {
                    var previewArrow = Instantiate(port.Way == Way.IN ? _inPreview : _outPreview, GridObjectView.transform);
                    
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
    }
}