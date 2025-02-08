using System;
using System.Collections.Generic;
using System.Linq;
using Components.Grid;
using Components.Ingredients;
using Components.Inventory;
using Components.Machines;
using Components.Machines.Behaviors;
using Database;
using UnityEngine;

namespace Components.Recipes.Grimoire
{
    public class UIRecipeController : MonoBehaviour
    {
        [SerializeField] private Transform _entriesHolder;
        [SerializeField] private RecipeEntryView _entryViewPrefab;
        
        private readonly List<RecipeEntryView> _allEntryViews = new();
        
        private void Awake()
        {
            var allRecipe = ScriptableObjectDatabase.GetAllScriptableObjectOfType<RecipeTemplate>();
            allRecipe = allRecipe.OrderBy(template => template.Ingredients.Count).ThenBy(template => template.OutIngredient.Name).ToList();
            
            foreach (var recipeTemplate in allRecipe)
            {
                var entryView = Instantiate(_entryViewPrefab, _entriesHolder);
                entryView.Initialize(recipeTemplate);
                
                _allEntryViews.Add(entryView);
            }
        }

        private void Start()
        {
            ResolutionFactoryState.OnResolutionFactoryStateStarted += HandleStartResolution;
        }

        private void HandleStartResolution(ResolutionFactoryState obj)
        {
            DisplayPotentialRecipes(true);
        }

        public void DisplayPotentialRecipes(bool display)
        {
            // Handle all entries (disable if we want to only show the potential recipes)
            for (int i = 0; i < _allEntryViews.Count; i++)
            {
                _allEntryViews[i].gameObject.SetActive(!display);
            }

            if (!display)
            {
                return;
            }

            if (GrimoireController.Instance.PlayerMachinesDictionary.Count == 0)
            {
                return;
            }

            // Get player machines
            var playerMachines = GrimoireController.Instance.PlayerMachinesDictionary.Keys.ToList();

            // Get extracted resources
            var playerResources = GridController.Instance.ExtractedIngredients;

            var potentialRecipes = ScriptableObjectDatabase.FindAllPotentialRecipes(playerMachines, playerResources);
            for (int i = 0; i < potentialRecipes.Count; i++)
            {
                // Activate the corresponding entry view for this recipe.
                var entryView = _allEntryViews.FirstOrDefault(entry => entry.RecipeName == potentialRecipes[i].name);
                if (entryView != null)
                {
                    entryView.gameObject.SetActive(true);
                }
            }
        }
    }
}