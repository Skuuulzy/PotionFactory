using System.Collections.Generic;
using Components.Ingredients;
using UnityEngine;

namespace Components.Machines
{
    public partial class MachineController
    {
        [Header("Arrow Preview")]
        [SerializeField] private GameObject _inPreview;
        [SerializeField] private GameObject _outPreview;
        
        [Header("Outline")]
        [SerializeField] private Color _placableColor = Color.green;
        [SerializeField] private Color _unPlacableColor = Color.red;
        [SerializeField] private Color _selectedColor = Color.blue;
        [SerializeField] private Color _hoveredColor = Color.white;
        
        private Outline _outline;
        private List<GameObject> _directionalArrows;

        
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
        
        // ------------------------------------------------------------------------- ITEM -----------------------------------------------------------------------------
        // TODO: The ingredient should not be controlled by the machine but need to be independent and linked to a machine.
        private void ShowItem(bool show)
        {
            if (!Machine.Template.ShowItem)
            {
                return;
            }
            
            if (show)
            {
                _ingredientController.CreateRepresentationFromTemplate(_machine.InIngredients);
            }
            else
            {
                _ingredientController.DestroyRepresentation();
            }
        }

        //TO Change : special for destructor behavior
        private void ShowItem(IngredientTemplate ingredient)
        {
            if (!Machine.Template.ShowItem)
            {
                return;
            }
            
            _ingredientController.CreateFavoriteSellerItemRepresentationFromTemplate(ingredient);
        }
    }
}