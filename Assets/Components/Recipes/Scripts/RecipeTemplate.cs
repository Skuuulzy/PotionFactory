using Components.Items;
using Components.Machines;
using UnityEngine;

namespace Components.Recipes
{
    [CreateAssetMenu(fileName = "New Recipe Template", menuName = "Recipes/Recipe Template")]
    public class RecipeTemplate : ScriptableObject
    {
        [SerializeField] private MachineTemplate _machine;
        [SerializeField] private SerializableDictionary<IngredientTemplate, int> _itemsUsedInRecipe;
        [SerializeField] private IngredientTemplate _outIngredientTemplate;

        public MachineTemplate Machine => _machine;
        public SerializableDictionary<IngredientTemplate, int> ItemsUsedInRecipe => _itemsUsedInRecipe;
        public IngredientTemplate OutIngredientTemplate => _outIngredientTemplate;
    }
}