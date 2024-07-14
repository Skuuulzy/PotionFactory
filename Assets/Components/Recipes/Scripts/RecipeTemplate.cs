using Components.Items;
using Components.Machines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Components.Recipes
{
    [CreateAssetMenu(fileName = "New Recipe Template", menuName = "Recipes/Recipe Template")]
    public class RecipeTemplate : ScriptableObject
    {
        [SerializeField] private MachineTemplate _machine;
        [SerializeField] private SerializableDictionary<ItemTemplate, int> _itemsUsedInRecipe;
        [SerializeField] private ItemTemplate _outItemTemplate;

        public MachineTemplate Machine => _machine;
        public SerializableDictionary<ItemTemplate, int> ItemsUsedInRecipe => _itemsUsedInRecipe;
        public ItemTemplate OutItemTemplate => _outItemTemplate;
    }
}

