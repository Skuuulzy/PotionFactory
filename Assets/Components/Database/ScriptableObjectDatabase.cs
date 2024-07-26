using System;
using System.Collections.Generic;
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

            T[] templates = Resources.LoadAll<T>(path);
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
                if (scriptableObject is not RecipeTemplate recipeTemplate)
                {
                    continue;
                }
                
                if (recipeTemplate.Machine != machineTemplate) 
                    continue;
				
                foreach (IngredientTemplate ingredient in inputsIngredients)
                {
                    if (!recipeTemplate.Ingredients.ContainsKey(ingredient))
                    {
                        continue;
                    }
					
                    recipe = recipeTemplate;
                    return true;
                }
            }

            //We don't find any recipes SO we return an unknown item recipe.
            recipe = GetScriptableObject<RecipeTemplate>("DEFAULT");
            return false;
        }
    }
}