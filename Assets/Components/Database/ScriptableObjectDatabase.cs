using System;
using System.Collections.Generic;
using System.Linq;
using Components.Items;
using Components.Machines;
using Components.Recipes;
using UnityEngine;

namespace Database
{
    public static class ScriptableObjectDatabase
    {
        private static readonly Dictionary<Type, Dictionary<string, ScriptableObject>> DATABASE = new();

        public const string INGREDIENTS_SO_PATH = "Components/Ingredients/Resources/";
        public const string RECIPES_SO_PATH = "Components/Recipes/Resources/";
        public const string MACHINE_SO_PATH = "Components/Machines/Resources/";

        
        // -------------------------------------- DATA BASE CONSTRUCTION ---------------------------------------------
        static ScriptableObjectDatabase()
        {
            // This static constructor will be called at the first time GetScriptableObject is called. Some nice lazy initialization here.
            LoadAllScriptableObjects<IngredientTemplate>(INGREDIENTS_SO_PATH);
            LoadAllScriptableObjects<RecipeTemplate>(RECIPES_SO_PATH);
            LoadAllScriptableObjects<MachineTemplate>(MACHINE_SO_PATH);
        }

        private static void LoadAllScriptableObjects<T>(string path) where T : ScriptableObject
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
            recipe = GetScriptableObject<RecipeTemplate>("DEFAULT");
            return false;
        }
    }
}