using Components.Items;
using Components.Recipes;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace VComponent.Tools.CSVReader
{
    public class CSVReaderOdinWindows : OdinMenuEditorWindow
    {
        private IngredientCreator _createIngredientCreator;

        [MenuItem("Tools/Items & Recipes database")]
        private static void OpenWindow()
        {
            GetWindow<CSVReaderOdinWindows>().Show();
            GetWindow<CSVReaderOdinWindows>().titleContent = new GUIContent("Ingredients & Recipes");
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree
            {
                Selection =
                {
                    SupportsMultiSelect = false
                }
            };

            _createIngredientCreator = new IngredientCreator();
            tree.Add("Ingredients", _createIngredientCreator);
            tree.AddAllAssetsAtPath("Ingredients", "Assets/Components/Ingredients/ScriptableObjects", typeof(IngredientTemplate));

            tree.AddAllAssetsAtPath("Recipes", "Assets/Components/Recipes/ScriptableObjects", typeof(RecipeTemplate));
            
            return tree;
        }

        protected override void OnBeginDrawEditors()
        {
            OdinMenuTreeSelection selected = MenuTree.Selection;

            SirenixEditorGUI.BeginHorizontalToolbar();
            {
                GUILayout.FlexibleSpace();

                if (SirenixEditorGUI.ToolbarButton("Delete Current"))
                {
                    IngredientTemplate asset = selected.SelectedValue as IngredientTemplate;
                    string path = AssetDatabase.GetAssetPath(asset);
                    AssetDatabase.DeleteAsset(path);
                    AssetDatabase.SaveAssets();
                }

            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (_createIngredientCreator != null)
            {
                // Clean the instance of the item template.
                DestroyImmediate(_createIngredientCreator.Ingredient);
            }
        }

        public class IngredientCreator
        {
            public const string INGREDIENTS_SO_PATH = "Assets/Components/Ingredients/ScriptableObjects/";
            
            // ------------------------------------ MANUAL CREATOR -------------------------------
            [BoxGroup("Manual"), InlineEditor(Expanded = true)] 
            public IngredientTemplate Ingredient;

            public IngredientCreator()
            {
                Ingredient = CreateInstance<IngredientTemplate>();
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
                    var isLiquid = csvDataLine[i][3] != "FALSE";
                    var executionTimeModifier = float.Parse(csvDataLine[i][6]);
                    
                    var ingredientTemplate = AssetDatabase.LoadAssetAtPath<IngredientTemplate>(IngredientPath(name));
                    
                    // If the asset does not exist create a new one
                    if (ingredientTemplate == null)
                    {
                        ingredientTemplate = CreateNewIngredientTemplate(CreateInstance<IngredientTemplate>(), name);
                    }
                    
                    ingredientTemplate.CreateFromCSV(name, price, isLiquid, executionTimeModifier);
                }
                
                AssetDatabase.SaveAssets();
            }

            private string IngredientPath(string ingredientName)
            {
                return INGREDIENTS_SO_PATH + ingredientName + ".asset";
            }
        }
    }
}