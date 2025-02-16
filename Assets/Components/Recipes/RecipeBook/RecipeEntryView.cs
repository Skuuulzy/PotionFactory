using System.Globalization;
using Components.Tick;
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
        [SerializeField] private TMP_Text _recipePriceTxt;
        [SerializeField] private TMP_Text _recipeTimeTxt;
        [SerializeField] private Image _recipeSprite;

        public string RecipeName { get; private set; }
        
        public void Initialize(RecipeTemplate recipeTemplate)
        {
            RecipeName = recipeTemplate.name;
            
            _recipeNameTxt.text = recipeTemplate.OutIngredient.Name;
            _recipeSprite.sprite = recipeTemplate.OutIngredient.Icon;
            _machineNameTxt.text = recipeTemplate.Machine.Name;
            _recipePriceTxt.text = $"{recipeTemplate.OutIngredient.Price}";
            var recipeTime = Mathf.RoundToInt(recipeTemplate.Machine.ProcessTime + recipeTemplate.ProcessTimeModifier) * TickSystem.Instance.InitialTickDuration;
            _recipeTimeTxt.text = recipeTime.ToString(CultureInfo.InvariantCulture);
            
            var ingredientsNeeded = string.Empty;
            
            foreach (var recipeTemplateIngredient in recipeTemplate.Ingredients)
            {
                ingredientsNeeded += $"{recipeTemplateIngredient.Key.Name} x{recipeTemplateIngredient.Value}, ";
            }
            _recipeIngredientsTxt.text = ingredientsNeeded;
        }
    }
}