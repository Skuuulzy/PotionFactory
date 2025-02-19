using Components.Ingredients;
using Database;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace VComponent.Tools.CSVReader
{
    public class IngredientsDatabaseManager
    {

        // ------------------------------------ MANUAL CREATOR -------------------------------
        [BoxGroup("Manual"), InlineEditor(Expanded = true)]
        public IngredientTemplate Ingredient;

        public IngredientsDatabaseManager()
        {
            Ingredient = ScriptableObject.CreateInstance<IngredientTemplate>();
        }

        [BoxGroup("Manual"), Button("Add New Ingredient")]
        private IngredientTemplate CreateNewIngredientTemplate(IngredientTemplate template, string name)
        {
            AssetDatabase.CreateAsset(template, IngredientPath(name));
            AssetDatabase.SaveAssets();

            return AssetDatabase.LoadAssetAtPath<IngredientTemplate>(IngredientPath(name));
        }

        // ------------------------------------ CSV -------------------------------
        [BoxGroup("From CSV")] 
        public TextAsset CSVFile;

        [BoxGroup("From CSV"), Button("Rebuild ingredient data base")]
        public void RebuildIngredientDataBase()
        {
            var csvDataLine = CSVReader.ReadCSV(CSVFile.name);

            for (int i = 0; i < csvDataLine.Count; i++)
            {
                // Parse the data
                var name = csvDataLine[i][0];
                var price = int.Parse(csvDataLine[i][1]);
                var nbOfTransformation = int.Parse(csvDataLine[i][2]);
                var isLiquid = csvDataLine[i][3] != "FALSE";
                var executionTimeModifier = float.Parse(csvDataLine[i][4]);

                var ingredientTemplate = GetIngredient(name);
                
                // If no ingredient is found create an instance
                if (ingredientTemplate == null)
                {
                    ingredientTemplate = CreateNewIngredientTemplate(ScriptableObject.CreateInstance<IngredientTemplate>(), name);
                }

                ingredientTemplate.CreateFromCSV(name, price, nbOfTransformation, isLiquid, executionTimeModifier);

                // Mark the asset as dirty to ensure Unity knows it has been modified
                EditorUtility.SetDirty(ingredientTemplate);
            }

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        public static IngredientTemplate GetIngredient(string name)
        {
            return ScriptableObjectDatabase.GetScriptableObject<IngredientTemplate>(name);
        }
        
        private static string IngredientPath(string ingredientName)
        {
            return ScriptableObjectDatabase.INGREDIENTS_SO_PATH + ingredientName + ".asset";
        }
    }
}