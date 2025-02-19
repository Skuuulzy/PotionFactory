using System.Collections.Generic;
using Components.Ingredients;
using Components.Machines;
using UnityEngine;

namespace Components.Recipes
{
    [CreateAssetMenu(fileName = "New Recipe Template", menuName = "Component/Recipes/Recipe Template")]
    public class RecipeTemplate : ScriptableObject
    {
        [SerializeField] private MachineTemplate _machine;
        [SerializeField] private SerializableDictionary<IngredientTemplate, int> _ingredients;
        [SerializeField] private IngredientTemplate _outIngredient;
        [SerializeField] private int _processTimeModifier;
        
        public MachineTemplate Machine => _machine;
        public Dictionary<IngredientTemplate, int> Ingredients => _ingredients.ToDictionary();
        public IngredientTemplate OutIngredient => _outIngredient;
        public int ProcessTimeModifier => _processTimeModifier;

        public void CreateFromCSV(IngredientTemplate outIngredient, MachineTemplate machine, Dictionary<IngredientTemplate, int> ingredients)
        {
            _outIngredient = outIngredient;
            _machine = machine;
            _ingredients = new SerializableDictionary<IngredientTemplate, int>(ingredients);

            // Compute the process modifier.
            _processTimeModifier = ComputeProcessTimeModifier(ingredients);
        }

        private int ComputeProcessTimeModifier(Dictionary<IngredientTemplate, int> ingredients)
        {
            var processTimeModifier = 0;
            foreach (var kvp in ingredients)
            {
                processTimeModifier += kvp.Key.ExecutionTimeModifier;
            }
            
            return processTimeModifier;
        }
    }
}