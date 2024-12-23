using System;
using System.Collections.Generic;
using System.Linq;
using Components.Bundle;
using Components.Grid.Decorations;
using Components.Grid.Obstacle;
using Components.Grid.Tile;
using Components.Ingredients;
using Components.Island;
using Components.Machines;
using Components.Recipes;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.WSA;

namespace Database
{
    public static class ScriptableObjectDatabase
    {
        private static readonly Dictionary<Type, Dictionary<string, ScriptableObject>> DATABASE = new();

        public const string INGREDIENTS_SO_PATH = "Components/Ingredients/Resources/";
        public const string RECIPES_SO_PATH = "Components/Recipes/Resources/";
        public const string MACHINE_SO_PATH = "Components/Machines/Resources/";
        public const string BUNDLE_SO_PATH = "Components/Bundles/Resources/";

        
        // -------------------------------------- DATA BASE CONSTRUCTION ---------------------------------------------
        static ScriptableObjectDatabase()
        {
            // This static constructor will be called at the first time GetScriptableObject is called. Some nice lazy initialization here.
            LoadAllScriptableObjects<IngredientsBundle>();
            LoadAllScriptableObjects<IngredientTemplate>();
            LoadAllScriptableObjects<RecipeTemplate>();
            LoadAllScriptableObjects<MachineTemplate>();
            LoadAllScriptableObjects<ConsumableTemplate>();
            LoadAllScriptableObjects<RelicTemplate>();
            LoadAllScriptableObjects<ObstacleTemplate>();
            LoadAllScriptableObjects<DecorationTemplate>();
            LoadAllScriptableObjects<Components.Grid.Tile.TileTemplate>();
            LoadAllScriptableObjects<IslandTemplate>();
        }

        private static void LoadAllScriptableObjects<T>() where T : ScriptableObject
        {
            var type = typeof(T);
            if (!DATABASE.ContainsKey(type))
            {
                DATABASE[type] = new Dictionary<string, ScriptableObject>();
            }

            T[] templates = Resources.LoadAll<T>("");
            foreach (var template in templates)
            {
                DATABASE[type][template.name] = template;
            }
        }

        public static T GetScriptableObject<T>(string name) where T : ScriptableObject
        {
            var type = typeof(T);
            if (DATABASE.TryGetValue(type, out var typeDictionary))
            {
                if (typeDictionary.TryGetValue(name, out var scriptableObject))
                {
                    return scriptableObject as T;
                }
            }
        
            Debug.LogError($"Unable to find a {type.Name} with name: {name}");
            return null;
        }

        public static List<T> GetAllScriptableObjectOfType<T>() where T : ScriptableObject
        {
            var result = new List<T>();

            var type = typeof(T);
            if (DATABASE.TryGetValue(type, out var typeDictionary))
            {
                foreach (var scriptableObject in typeDictionary)
                {
                    result.Add(scriptableObject.Value as T);
                }
            }

            return result;
        }
        
        // ----------------------------------------- RECIPE DATA BASE ------------------------------------------
        public static bool TryFindRecipe(MachineTemplate machineTemplate, List<IngredientTemplate> inputsIngredients, out RecipeTemplate recipe)
        {
            foreach (var scriptableObject in DATABASE[typeof(RecipeTemplate)].Values)
            {
                if (scriptableObject is not RecipeTemplate currentRecipeTemplate)
                {
                    continue;
                }

                if (currentRecipeTemplate.Machine != machineTemplate)
                    continue;

                bool isRecipeValid = true;
                
                // We go through every ingredient of the recipe.
                foreach (var recipeIngredient in currentRecipeTemplate.Ingredients)
                {
                    // We get the number of ingredients of that type in the inputs ingredients.
                    int inputIngredientTypeCount = inputsIngredients.Count(ingredient => ingredient.Name == recipeIngredient.Key.Name);

                    // If there is not enough ingredients, the recipe cannot be made.
                    if (inputIngredientTypeCount < recipeIngredient.Value)
                    {
                        isRecipeValid = false;
                        break;
                    }
                }

                if (isRecipeValid)
                {
                    recipe = currentRecipeTemplate;
                    return true;
                }
            }

            // If no recipe was found, return the default.
            recipe = null;
            return false;
        }
    }
}