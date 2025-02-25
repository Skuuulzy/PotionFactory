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
        [SerializeField] private GameObject _recipeBookPanel;
        
        private List<RecipeEntryView> _allEntryViews = new();
        private List<RecipeTemplate> _allRecipes;
        private void Awake()
        {
            _allRecipes = ScriptableObjectDatabase.GetAllScriptableObjectOfType<RecipeTemplate>();
        }


        private void SetRecipesOrder()
        {
            var potentialRecipes = GetPotentialRecipes();
            _allRecipes = _allRecipes.OrderByDescending(template => potentialRecipes.Contains(template)).ThenBy(template => template.OutIngredient.Price).ThenBy(template => template.OutIngredient.Name).ToList();
            _allEntryViews = new();

            foreach (Transform child in _entriesHolder)
            {
                Destroy(child.gameObject);
            }

            foreach (var recipeTemplate in _allRecipes)
            {
                var entryView = Instantiate(_entryViewPrefab, _entriesHolder);
                entryView.Initialize(recipeTemplate);
                entryView.SetState(potentialRecipes.Contains(recipeTemplate));
                _allEntryViews.Add(entryView);
            }
        }
        public void DisplayRecipeBook(bool value)
        {
            _recipeBookPanel.SetActive(value);
            if (value)
            {
                SetRecipesOrder();
            }
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

            if (InventoryController.Instance.PlayerMachinesDictionary.Count == 0)
            {
                return;
            }

            var potentialRecipes = GetPotentialRecipes();

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

        private List<RecipeTemplate> GetPotentialRecipes()
        {
            // Get player machines
            var playerMachines = InventoryController.Instance.PlayerMachinesDictionary.Keys.ToList();

            // Get extracted resources
            var playerResources = GridController.Instance.ExtractedIngredients;

            var potentialRecipes = ScriptableObjectDatabase.FindAllPotentialRecipes(playerMachines, playerResources);
            return potentialRecipes;
        }
    }
}