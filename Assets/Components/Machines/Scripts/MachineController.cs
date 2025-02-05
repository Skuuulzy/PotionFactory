using System.Collections.Generic;
using UnityEngine;

namespace Components.Machines
{
    public class MachineController : MonoBehaviour
    {
        [Header("Holders")]
        [SerializeField] private Transform _3dViewHolder;
        
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
        
        private Outline _outline;
        private List<GameObject> _directionalArrows;
        
        private GameObject _view;
        private List<MachineGridComponent> _gridComponents = new();
        
        private bool _initialized;
        private bool _selected;
        
        public Machine Machine => _machine;
        
        // ------------------------------------------------------------------------- INIT -----------------------------------------------------------------------------
        public void InstantiatePreview(MachineTemplate machineTemplate, float scale, bool showOutlines = false)
        {
            _view = Instantiate(machineTemplate.GridView, _3dViewHolder);
            _machine = new Machine(machineTemplate, this);
            _view.transform.localScale = new Vector3(scale, scale, scale);

            SetupDirectionalArrows(machineTemplate);

            _outline = _view.AddComponent<Outline>();
            _outline.OutlineWidth = 8;
            ToggleOutlines(showOutlines, _placableColor);
            
            // Instantiate grid components
            for (int i = 0; i < _machine.Template.GridComponents.Count; i++)
            {
                var gridComponent = Instantiate(_machine.Template.GridComponents[i], transform);
                gridComponent.Initialize(_machine);
                _gridComponents.Add(gridComponent);
            }
        }
        
        public void RotatePreview(int angle)
        {
            _view.transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
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
            if (_selected)
            {
                return;
            }
            
            if (Machine != machine)
            {
                ToggleOutlines(false);
                return;
            }
            
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
                    var previewArrow = Instantiate(port.Way == Way.IN ? _inPreview : _outPreview, _view.transform);
                    
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