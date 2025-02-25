using Components.Ingredients;
using Components.Machines;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Components.Shop.ShopItems
{
    public class ShopItem 
    {
        // ----------------------------------------------------------------------- PRIVATE FIELDS -------------------------------------------------------------------------
        private MachineTemplate _machineTemplate;

        private IngredientTemplate _ingredientTemplate;
        private float _spawnProbability;
        private int _numberOfItemToSell;

        // ----------------------------------------------------------------------- PUBLIC FIELDS -------------------------------------------------------------------------
        public MachineTemplate MachineTemplate => _machineTemplate;
        public IngredientTemplate IngredientTemplate => _ingredientTemplate;
        public float SpawnProbability => _spawnProbability;
        public int NumberOfItemToSell => _numberOfItemToSell;

        public ShopItem(MachineTemplate machineTemplate, int numberOfItemToSell = 1)
        {
            _machineTemplate = machineTemplate;
            _spawnProbability = machineTemplate.ShopSpawnProbability;
            _numberOfItemToSell = numberOfItemToSell;
        }

        public ShopItem(IngredientTemplate ingredientTemplate, int numberOfItemToSell = 1)
        {
            _ingredientTemplate = ingredientTemplate;
            _numberOfItemToSell = numberOfItemToSell;
        }

		public void DecreaseNumberOfItemToSell()
        {
            _numberOfItemToSell--;
        }
    }
}
