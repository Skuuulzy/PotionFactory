using Components.Economy;
using Components.Ingredients;
using UnityEngine;

namespace Components.Machines.Behaviors
{
    [CreateAssetMenu(fileName = "New Machine Behaviour", menuName = "Machines/Behavior/Destructor")]
    public class DestructorMachineBehaviour : MachineBehavior
    {
        public override void Process(Machine machine)
        {
            // Sell items
            int sellPrice = 0;
            foreach(IngredientTemplate ingredientTemplate in machine.Ingredients)
            {
                sellPrice += ingredientTemplate.Price;
            }
            
            EconomyController.Instance.AddMoney(sellPrice);
            
            // Clear the machine items
            machine.RemoveAllItems();
        }
    }
}