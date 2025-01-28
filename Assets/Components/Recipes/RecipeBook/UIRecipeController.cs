using System.Collections.Generic;
using System.Linq;
using Components.Grid;
using Components.Inventory;
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
            
            // Get player machines
            var playerMachines = GrimoireController.Instance.PlayerMachinesDictionary.Keys.ToList();
            
            // Get extracted resources
            var playerResources = GridController.Instance.ExtractedIngredients;
            
            for (int i = 0; i < playerMachines.Count; i++)
            {
                var machine = playerMachines[i];

                // We only check machine capable of creating resources (conveyor, merger, ... are excluded)
                if (machine.GetBehaviorClone().GetType() != typeof(RecipeCreationBehavior))
                {
                    continue;
                }
                
                Debug.Log($"Trying to find potential recipe with machine: {machine.Name}, and ingredients:");
                for (int j = 0; j < playerResources.Count; j++)
                {
                    Debug.Log($"{playerResources[j].Name}");
                }
                
                if (ScriptableObjectDatabase.TryFindRecipe(machine, playerResources, out var recipe))
                {
                    // TODO: This will need some optimisation in the future.
                    _allEntryViews.First(entry => entry.RecipeName == recipe.name).gameObject.SetActive(true);
                    Debug.Log($"Recipe found: {recipe.name}");
                }
            }
        }
    }
}