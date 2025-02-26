using System.Collections.Generic;
using System.Linq;
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

        [Header("Placed Object")] 
        [SerializeField, Tooltip("Object that are un active on preview and activated when the object is placed.")] 
        private List<GameObject> _placedObject;
        
        private static readonly int GHOST_COLOR = Shader.PropertyToID("_Ghost_Color");
        private Dictionary<Renderer,List<Material>> _originalMaterials;
        private List<Material> _bluePrintMaterialInstances;
        private bool _outlineSetup;

        private void Awake()
        {
            SetupBlueprintRenderers();
            SetupOutlines();
        }
        
        // ------------------------------------------------------------------------- PLACED OBJECT -------------------------------------------------------------------------

        private void TogglePlacedObject(bool toggle)
        {
            for (int i = 0; i < _placedObject.Count; i++)
            {
                _placedObject[i].SetActive(toggle);
            }
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
                var materialsToApply = new List<Material>();

                if (toggle)
                {
                    // We need to apply the same amount of materials if the renderer have multiple materials.
                    for (int i = 0; i < _originalMaterials[machineRenderer].Count; i++)
                    {
                        materialsToApply.Add(_bluePrintMaterial);
                    }
                }
                else
                {
                    materialsToApply = _originalMaterials[machineRenderer];
                }
                
                // Applying materials
                machineRenderer.SetMaterials(materialsToApply);
                
                // Caching used blueprint materials to update their colors.
                if (toggle)
                {
                    var materialsInstances = new List<Material>();
                    machineRenderer.GetMaterials(materialsInstances);
                    _bluePrintMaterialInstances.AddRange(materialsInstances);
                    
                    // If we toggle the blueprints materials, set the first state as placable.
                    UpdateBlueprintColor(true);
                    TogglePlacedObject(false);
                    ToggleHoverOutlines(false);
                }
                else
                {
                    _bluePrintMaterialInstances.Clear();
                    TogglePlacedObject(true);
                }
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