using System.Collections.Generic;
using System.Linq;
using Components.Ingredients;
using Components.Machines.Behaviors;
using Database;
using TMPro;
using UnityEngine;

namespace Components.Machines.UIView
{
    public class ExtractorContextualUIView : UIContextualComponent
    {
        [SerializeField] private TMP_Dropdown _extractorResourceTypeDropdown;

        private Machine _associatedMachine;
        private List<IngredientTemplate> _allIngredients;

        public override void Initialize(Machine machine)
        {
            _associatedMachine = machine;
            
            _allIngredients = ScriptableObjectDatabase.GetAllScriptableObjectOfType<IngredientTemplate>();
            var ingredientsName = _allIngredients.Select(ingredientTemplate => ingredientTemplate.Name).ToList();
            ingredientsName.Insert(0,"NONE");

            _extractorResourceTypeDropdown.ClearOptions();
            _extractorResourceTypeDropdown.AddOptions(ingredientsName);

            // Set up the dropdown to the selected extractor ingredient if there is one.
            var machineBehavior = _associatedMachine.Behavior;
            if (machineBehavior is ExtractorMachineBehaviour extractorMachineBehaviour)
            {
                if (extractorMachineBehaviour.IngredientToExtract != null)
                { 
                    // +1 because there is the NONE at index 0.
                    _extractorResourceTypeDropdown.SetValueWithoutNotify(_allIngredients.IndexOf(extractorMachineBehaviour.IngredientToExtract) + 1);
                }
            }
        }

        public void SelectIngredient(int ingredientIndex)
        {
            // NONE selected
            if (ingredientIndex == 0)
            {
                SetSelectedIngredient(null);
                return;
            }
            
            // -1 because there is the NONE at index 0.
            var selectedIngredient = _allIngredients[ingredientIndex - 1];
            SetSelectedIngredient(selectedIngredient);
        }

        private void SetSelectedIngredient(IngredientTemplate template)
        {
            var machineBehavior = _associatedMachine.Behavior;
            if (machineBehavior is ExtractorMachineBehaviour extractorMachineBehaviour)
            {
                extractorMachineBehaviour.SetExtractedIngredient(template);
            }
        }
    }
}