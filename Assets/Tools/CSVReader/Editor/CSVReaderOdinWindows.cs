using Components.Items;
using Components.Recipes;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace VComponent.Tools.CSVReader
{
    public partial class CSVReaderOdinWindows : OdinMenuEditorWindow
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

            tree.Add("Recipes", new RecipesCreator());
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
    }
}