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
        private ConsumableTemplate _consumableTemplate;
        private RelicTemplate _relicTemplate;
        private IngredientTemplate _ingredientTemplate;
        private float _spawnProbability;
        private int _numberOfItemToSell;



        // ----------------------------------------------------------------------- PUBLIC FIELDS -------------------------------------------------------------------------
        public MachineTemplate MachineTemplate => _machineTemplate;
        public ConsumableTemplate ConsumableTemplate => _consumableTemplate;
        public RelicTemplate RelicTemplate => _relicTemplate;
        public IngredientTemplate IngredientTemplate => _ingredientTemplate;
        public float SpawnProbability => _spawnProbability;
        public int NumberOfItemToSell => _numberOfItemToSell;

        public ShopItem(MachineTemplate machineTemplate, int numberOfItemToSell = 1)
        {
            _machineTemplate = machineTemplate;
            _spawnProbability = machineTemplate.ShopSpawnProbability;
            _numberOfItemToSell = numberOfItemToSell;
        }

        public ShopItem(ConsumableTemplate consumableTemplate, int numberOfItemToSell = 1)
        {
            _consumableTemplate = consumableTemplate;
			_numberOfItemToSell = numberOfItemToSell;
            _spawnProbability = consumableTemplate.SpawnProbability;
		}

		public ShopItem(RelicTemplate relicTemplate, int numberOfItemToSell = 1)
		{
			_relicTemplate = relicTemplate;
			_numberOfItemToSell = numberOfItemToSell;
            _spawnProbability = relicTemplate.SpawnProbability;
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
