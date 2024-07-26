using System.Collections.Generic;
using Components.Items;
using Components.Recipes;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace VComponent.Tools.CSVReader
{
    public class RecipesCreator
    {
        private const string RECIPES_SO_PATH = "Assets/Components/Recipes/ScriptableObjects/";
        private readonly List<int> _ingredientsCSVIndexes = new List<int>() { 5, 8, 11 };

        private RecipeTemplate CreateNewRecipeTemplate(RecipeTemplate template, string name)
        {
            AssetDatabase.CreateAsset(template, RecipePath(name));
            AssetDatabase.SaveAssets();

            return AssetDatabase.LoadAssetAtPath<RecipeTemplate>(RecipePath(name));
        }

        // ------------------------------------ CSV -------------------------------
        [BoxGroup("From CSV")] public TextAsset CSVFile;

        [BoxGroup("From CSV"), Button("Rebuild recipes data base")]
        public void RebuildRecipesDataBase()
        {
            var csvDataLine = CSVReader.ReadCSV(CSVFile.name);

            for (int i = 0; i < csvDataLine.Count; i++)
            {
                // Parse the data
                var name = csvDataLine[i][0];
                var outIngredient = IngredientCreator.GetIngredient(name);
                var machine = MachineCreator.GetMachine(csvDataLine[i][3]);

                // Find recipes ingredients
                Dictionary<IngredientTemplate, int> recipesIngredients = new();
                for (int j = 0; j < 3; j++)
                {
                    var ingredientName = csvDataLine[i][_ingredientsCSVIndexes[j]];
                    
                    // The recipe only have one or two ingredients no need to go further.
                    if (ingredientName == string.Empty)
                    {
                        break;
                    }

                    IngredientTemplate ingredient = IngredientCreator.GetIngredient(ingredientName);
                    
                    // No ingredients have been found
                    if (!ingredient)
                    {
                        Debug.LogError($"Unable to find the scriptable object associated with the ingredient: {ingredientName}");
                        continue;
                    }
                    
                    int ingredientCount = int.Parse(csvDataLine[i][_ingredientsCSVIndexes[j] - 1]);
                    
                    recipesIngredients.Add(ingredient, ingredientCount);
                }

                var recipeTemplate = GetRecipe(name);

                // If the asset does not exist create a new one
                if (recipeTemplate == null)
                {
                    recipeTemplate = CreateNewRecipeTemplate(ScriptableObject.CreateInstance<RecipeTemplate>(), name);
                }

                recipeTemplate.CreateFromCSV(outIngredient, machine, recipesIngredients);

                // Mark the asset as dirty to ensure Unity knows it has been modified
                EditorUtility.SetDirty(recipeTemplate);
            }

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
        
        public static RecipeTemplate GetRecipe(string name)
        {
            return AssetDatabase.LoadAssetAtPath<RecipeTemplate>(RecipePath(name));
        }

        private static string RecipePath(string ingredientName)
        {
            return RECIPES_SO_PATH + ingredientName + ".asset";
        }
    }
}