using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Components.Recipes.Grimoire
{
    public class RecipeEntryView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _recipeNameTxt;
        [SerializeField] private TMP_Text _machineNameTxt;
        [SerializeField] private TMP_Text _recipeIngredientsTxt;
        [SerializeField] private Image _recipeSprite;

        public string RecipeName { get; private set; }
        
        public void Initialize(RecipeTemplate recipeTemplate)
        {
            RecipeName = recipeTemplate.name;
            
            _recipeNameTxt.text = recipeTemplate.OutIngredient.Name;
            _recipeSprite.sprite = recipeTemplate.OutIngredient.Icon;
            _machineNameTxt.text = recipeTemplate.Machine.Name;

            var ingredientsNeeded = string.Empty;
            
            foreach (var recipeTemplateIngredient in recipeTemplate.Ingredients)
            {
                ingredientsNeeded += $"{recipeTemplateIngredient.Key.Name} x{recipeTemplateIngredient.Value}, ";
            }
            _recipeIngredientsTxt.text = ingredientsNeeded;
        }
    }
}