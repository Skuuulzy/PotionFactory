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
using Components.Machines.Behaviors;
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
            LoadAllScriptableObjects<IngredientsBundle>();
            LoadAllScriptableObjects<IngredientTemplate>();
            LoadAllScriptableObjects<RecipeTemplate>();
            LoadAllScriptableObjects<MachineTemplate>();
            LoadAllScriptableObjects<ConsumableTemplate>();
            LoadAllScriptableObjects<RelicTemplate>();
            LoadAllScriptableObjects<ObstacleTemplate>();
            LoadAllScriptableObjects<DecorationTemplate>();
            LoadAllScriptableObjects<TileTemplate>();
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
        
        // ----------------------------------------- RECIPES ------------------------------------------
        
        /// With a precise list of ingredients and a machine, check if a recipe can be made.
        public static bool TryFindRecipeMachine(MachineTemplate machineTemplate, List<IngredientTemplate> inputsIngredients, out RecipeTemplate recipe)
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
        
        /// Finds all potential recipes with a list of machines and ingredients, considering transformed ingredients.
        public static List<RecipeTemplate> FindAllPotentialRecipes(List<MachineTemplate> playerMachines, List<IngredientTemplate> playerResources)
        {
            List<RecipeTemplate> foundRecipes = new List<RecipeTemplate>();
            
            // Track of all discovered resources (base + transformed).
            HashSet<IngredientTemplate> allAvailableResources = new HashSet<IngredientTemplate>(playerResources);
            
            // Track already discovered recipes.
            HashSet<string> processedRecipes = new HashSet<string>();

            bool foundNewRecipes;
            do
            {
                foundNewRecipes = false;

                // Iterate through each machine to check recipes it can create.
                for (int i = 0; i < playerMachines.Count; i++)
                {
                    var machine = playerMachines[i];

                    // Skip machine that cannot create recipes.
                    switch (machine.Type)
                    {
                        case MachineType.CONVEYOR:
                        case MachineType.MARCHAND:
                        case MachineType.EXTRACTOR:
                        case MachineType.MERGER:
                        case MachineType.SPLITTER:
                            continue;
                    }
                    
                    // Get all potential recipes the machine can create with the current list of resources.
                    var machineRecipes = FindMachinePotentialRecipe(machine, allAvailableResources.ToList());

                    foreach (var recipe in machineRecipes)
                    {
                        // Skip already processed recipes to avoid redundant work.
                        if (!processedRecipes.Add(recipe.name))
                        {
                            continue;
                        }

                        foundNewRecipes = true;
                        foundRecipes.Add(recipe);

                        // Add the out ingredient of the recipes to the pool of ingredients.
                        allAvailableResources.Add(recipe.OutIngredient);
                    }
                }
            } while (foundNewRecipes); // Continue until no new recipes are found.

            return foundRecipes;
        }

        /// With a list of ingredients and a machine, find potentials recipes. Do not take in account the quantity of ingredients.
        public static List<RecipeTemplate> FindMachinePotentialRecipe(MachineTemplate machineTemplate, List<IngredientTemplate> inputsIngredients)
        {
            var recipes = new List<RecipeTemplate>();

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
                    if (!inputsIngredients.Contains(recipeIngredient.Key))
                    {
                        isRecipeValid = false;
                        break;
                    }
                }

                if (isRecipeValid)
                {
                    recipes.Add(currentRecipeTemplate);
                }
            }

            return recipes;
        }
        
        // ----------------------------------------- TILES ------------------------------------------
       
        public static TileTemplate GetTileTemplateByType(TileType type)
        {
            switch (type)
            {
                case TileType.GRASS:
                    return GetScriptableObject<TileTemplate>("GrassTile");
                case TileType.WATER:
                    return GetScriptableObject<TileTemplate>("WaterTile");
                case TileType.SAND:
                    return GetScriptableObject<TileTemplate>("SandTile");
                case TileType.STONE:
                    return GetScriptableObject<TileTemplate>("StoneTile");
                case TileType.DIRT:
                    return GetScriptableObject<TileTemplate>("DirtTile");
                case TileType.NONE:
                default:
                    Debug.LogError($"Unknown tile type: {type}, returning GRASS.");
                    return GetScriptableObject<TileTemplate>("GrassTile");
            }
        }
        
        // ----------------------------------------- OBSTACLES ------------------------------------------

        public static ObstacleTemplate GetObstacleTemplateByType(ObstacleType type)
        {
            var obstaclesTemplates = GetAllScriptableObjectOfType<ObstacleTemplate>();

            if (obstaclesTemplates.Count == 0)
            {
                Debug.LogError("No obstacles templates found in data base.");
                return null;
            }

            for (int i = 0; i < obstaclesTemplates.Count; i++)
            {
                var obstacleTemplate = obstaclesTemplates[i];
                if (obstacleTemplate.ObstacleType == type)
                {
                    return obstacleTemplate;
                }
            }
            
            Debug.LogError($"Unable to find obstacle of type {type} in the database. Returning first found.");
            return obstaclesTemplates[0];
        }
        
        // ----------------------------------------- DECORATIONS ------------------------------------------

        public static DecorationTemplate GetDecorationTemplateByType(DecorationType type)
        {
            var decorationTemplates = GetAllScriptableObjectOfType<DecorationTemplate>();

            if (decorationTemplates.Count == 0)
            {
                Debug.LogError("No obstacles templates found in data base.");
                return null;
            }

            for (int i = 0; i < decorationTemplates.Count; i++)
            {
                var decorationTemplate = decorationTemplates[i];
                if (decorationTemplate.DecorationType == type)
                {
                    return decorationTemplate;
                }
            }
            
            Debug.LogError($"Unable to find decoration of type {type} in the database. Returning first found.");
            return decorationTemplates[0];
        }
    }
}