using System.Collections.Generic;
using Components.Items;
using Components.Machines;
using UnityEngine;

namespace Components.Recipes
{
    [CreateAssetMenu(fileName = "New Recipe Template", menuName = "Recipes/Recipe Template")]
    public class RecipeTemplate : ScriptableObject
    {
        [SerializeField] private MachineTemplate _machine;
        [SerializeField] private SerializableDictionary<IngredientTemplate, int> _ingredients;
        [SerializeField] private IngredientTemplate _outIngredient;

        public MachineTemplate Machine => _machine;
        public Dictionary<IngredientTemplate, int> Ingredients => _ingredients.ToDictionary();
        public IngredientTemplate OutIngredient => _outIngredient;

        public void CreateFromCSV(IngredientTemplate outIngredient, MachineTemplate machine, Dictionary<IngredientTemplate, int> ingredients)
        {
            _outIngredient = outIngredient;
            _machine = machine;
            _ingredients = new SerializableDictionary<IngredientTemplate, int>(ingredients);
        }
    }
}