using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Database;
using UnityEngine;

namespace Components.Recipes.Grimoire
{
    public class UIRecipeController : MonoBehaviour
    {
        [SerializeField] private Transform _entriesHolder;
        [SerializeField] private RecipeEntryView _entryViewPrefab;
        
        private List<RecipeEntryView> _entryViews;
        
        private void Awake()
        {
            var allRecipe = ScriptableObjectDatabase.GetAllScriptableObjectOfType<RecipeTemplate>();
            allRecipe = allRecipe.OrderBy(template => template.Ingredients.Count).ThenBy(template => template.OutIngredient.Name).ToList();
            
            _entryViews = new List<RecipeEntryView>();
            
            foreach (var recipeTemplate in allRecipe)
            {
                var entryView = Instantiate(_entryViewPrefab, _entriesHolder);
                entryView.Initialize(recipeTemplate);
                
                _entryViews.Add(entryView);
            }
        }
    }
}