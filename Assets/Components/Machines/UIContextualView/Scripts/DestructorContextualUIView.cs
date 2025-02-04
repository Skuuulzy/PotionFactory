using Components.Ingredients;
using Components.Machines.Behaviors;
using Database;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Components.Machines.UIView
{
    public class DestructorContextualUIView : UIContextualComponent
    {
        [SerializeField] private TMP_Dropdown _destructorResourceTypeDropdown;

        private Machine _associatedMachine;
        private List<IngredientTemplate> _allIngredients;

        public override void Initialize(Machine machine)
        {
            _associatedMachine = machine;

            _allIngredients = ScriptableObjectDatabase.GetAllScriptableObjectOfType<IngredientTemplate>();
            var ingredientsName = _allIngredients.Select(ingredientTemplate => ingredientTemplate.Name).ToList();
            ingredientsName.Insert(0, "NONE");

            _destructorResourceTypeDropdown.ClearOptions();
            _destructorResourceTypeDropdown.AddOptions(ingredientsName);

            // Set up the dropdown to the selected extractor ingredient if there is one.
            var machineBehavior = _associatedMachine.Behavior;
            if (machineBehavior is MarchandMachineBehaviour destructorMachineBehaviour)
            {
                if (destructorMachineBehaviour.FavoriteIngredient != null)
                {
                    // +1 because there is the NONE at index 0.
                    _destructorResourceTypeDropdown.SetValueWithoutNotify(_allIngredients.IndexOf(destructorMachineBehaviour.FavoriteIngredient) + 1);
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
            if (machineBehavior is MarchandMachineBehaviour destructorMachineBehaviour)
            {
                destructorMachineBehaviour.SetFavoriteIngredient(template);
            }
        }
    }
}
