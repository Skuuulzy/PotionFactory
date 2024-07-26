using Components.Items;
using Components.Machines;
using Components.Recipes;
using Database;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace VComponent.Tools.CSVReader
{
    public class CSVReaderOdinWindows : OdinMenuEditorWindow
    {
        private IngredientsDatabaseManager _createIngredientsDatabaseManager;

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

            _createIngredientsDatabaseManager = new IngredientsDatabaseManager();
            tree.Add("Ingredients", _createIngredientsDatabaseManager);
            tree.AddAllAssetsAtPath("Ingredients", ScriptableObjectDatabase.INGREDIENTS_SO_PATH, typeof(IngredientTemplate));

            tree.Add("Recipes", new RecipesDatabaseManager());
            tree.AddAllAssetsAtPath("Recipes", ScriptableObjectDatabase.RECIPES_SO_PATH, typeof(RecipeTemplate));
            
            tree.Add("Machines", new MachinesDatabaseManager());
            tree.AddAllAssetsAtPath("Machines", ScriptableObjectDatabase.MACHINE_SO_PATH, typeof(MachineTemplate));
            
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

            if (_createIngredientsDatabaseManager != null)
            {
                // Clean the instance of the item template.
                DestroyImmediate(_createIngredientsDatabaseManager.Ingredient);
            }
        }
    }
}