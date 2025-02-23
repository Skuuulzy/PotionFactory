using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Components.Grid
{
    public class GridObjectViewController : MonoBehaviour
    {
        [Header("Outline")] 
        [SerializeField] private Outline _outline;
        [SerializeField] private float _outlineWidth = 8;
        [SerializeField] private Color _outlineColor = Color.white;
        [SerializeField] private List<Renderer> _renderersToOutline;
        
        [Header("Blueprint")]
        [SerializeField] private Material _bluePrintMaterial;
        [SerializeField] private Color _placableColor = Color.green;
        [SerializeField] private Color _unPlacableColor = Color.red;
        [SerializeField] private List<Renderer> _renderersToBlueprint;
        
        private static readonly int GHOST_COLOR = Shader.PropertyToID("_Ghost_Color");
        private Dictionary<Renderer,List<Material>> _originalMaterials;
        private List<Material> _bluePrintMaterialInstances;
        private bool _outlineSetup;

        private void Awake()
        {
            SetupBlueprintRenderers();
            SetupOutlines();
        }

        // ------------------------------------------------------------------------- BLUEPRINTS -----------------------------------------------------------------------------
        
        private void SetupBlueprintRenderers()
        {
            if (_renderersToBlueprint.Count == 0)
            {
                return;
            }

            // Save associated renderers materials
            _originalMaterials = new Dictionary<Renderer, List<Material>>();
            
            foreach (var bluePrintRenderer in _renderersToBlueprint)
            {
                _originalMaterials.Add(bluePrintRenderer, bluePrintRenderer.materials.ToList());
            }
        }
        
        public void ToggleBlueprintMaterials(bool toggle)
        {
            _bluePrintMaterialInstances = new List<Material>();
            
            foreach (var machineRenderer in _renderersToBlueprint)
            {
                List<Material> materialsToApply = toggle ? new List<Material> {_bluePrintMaterial} : _originalMaterials[machineRenderer];
                machineRenderer.SetMaterials(materialsToApply);
                
                // Caching blueprint materials
                var materialsInstances = new List<Material>();
                machineRenderer.GetMaterials(materialsInstances);
                _bluePrintMaterialInstances.AddRange(materialsInstances);
            }

            // If we toggle the blueprints materials, set the first state as placable.
            if (toggle)
            {
                UpdateBlueprintColor(true);
            }
        }
        
        public void UpdateBlueprintColor(bool placable)
        {
            foreach (var bluePrintMaterial in _bluePrintMaterialInstances)
            {
                bluePrintMaterial.SetColor(GHOST_COLOR, placable ? _placableColor : _unPlacableColor);
            }
        }

        // ------------------------------------------------------------------------- OUTLINES -----------------------------------------------------------------------------
        
        private void SetupOutlines()
        {
            if (!_outline.Initialized(_renderersToOutline)) 
                return;
            
            _outline.OutlineWidth = _outlineWidth;
            _outline.OutlineColor = _outlineColor;
            _outlineSetup = true;
            
            ToggleHoverOutlines(false);
        }
        
        public void ToggleHoverOutlines(bool toggle)
        {
            if (_outlineSetup)
            {
                _outline.enabled = toggle;
            }
        }
    }
}