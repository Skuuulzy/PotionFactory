using System.Collections.Generic;
using System.Linq;
using Components.Items;
using Components.Machines.Behaviors;
using Database;
using TMPro;
using UnityEngine;

namespace Components.Machines.UIView
{
    public class ExtractorUIView : MachineUIViewBase
    {
        [SerializeField] private TMP_Dropdown _extractorResourceTypeDropdown;

        private List<IngredientTemplate> _allIngredients;
        
        public override void Initialize(Machine machine)
        {
            base.Initialize(machine);
            
            _allIngredients = ScriptableObjectDatabase.GetAllScriptableObjectOfType<IngredientTemplate>();
            var ingredientsName = _allIngredients.Select(ingredientTemplate => ingredientTemplate.Name).ToList();
            ingredientsName.Insert(0,"NONE");

            _extractorResourceTypeDropdown.ClearOptions();
            _extractorResourceTypeDropdown.AddOptions(ingredientsName);
        }

        public void SelectIngredient(int ingredientIndex)
        {
            // NONE selected
            if (ingredientIndex == 0)
            {
                SetSelectedIngredient(null);
                return;
            }
            
            // +1 because there is the NONE at index 0.
            var selectedIngredient = _allIngredients[ingredientIndex - 1];
            SetSelectedIngredient(selectedIngredient);
        }

        private void SetSelectedIngredient(IngredientTemplate template)
        {
            var machineBehavior = AssociatedMachine.Behavior;
            if (machineBehavior is ExtractorMachineBehaviour extractorMachineBehaviour)
            {
                extractorMachineBehaviour.Init(template);
            }
        }
    }
}