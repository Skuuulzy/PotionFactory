using Components.Economy;
using Components.Ingredients;
using UnityEngine;

namespace Components.Machines.Behaviors
{
    [CreateAssetMenu(fileName = "New Machine Behaviour", menuName = "Machines/Behavior/Destructor")]
    public class DestructorMachineBehaviour : MachineBehavior
    {
        private IngredientTemplate _specialIngredientTemplate;

        public IngredientTemplate SpecialIngredientTemplate => _specialIngredientTemplate;
        public override void Process(Machine machine)
        {
            // Sell items
            int sellPrice = 0;
            foreach(IngredientTemplate ingredientTemplate in machine.Ingredients)
            {
                if(ingredientTemplate != null && ingredientTemplate.Name == _specialIngredientTemplate.Name)
                {
                    sellPrice += ingredientTemplate.Price * 2;
                }
                else
                {
					sellPrice += ingredientTemplate.Price;
				}             
            }
            
            EconomyController.Instance.AddMoney(sellPrice);
            
            // Clear the machine items
            machine.RemoveAllItems();
        }

        public void SetSpecialIngredientTemplate(IngredientTemplate specialIngredient)
        {
            _specialIngredientTemplate = specialIngredient;
        }
    }
}