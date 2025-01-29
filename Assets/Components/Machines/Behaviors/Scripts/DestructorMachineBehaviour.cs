using Components.Economy;
using Components.Ingredients;
using System;
using UnityEngine;

namespace Components.Machines.Behaviors
{
    [CreateAssetMenu(fileName = "New Machine Behaviour", menuName = "Component/Machines/Behavior/Destructor")]
    public class DestructorMachineBehaviour : MachineBehavior
    {
        private IngredientTemplate _specialIngredientTemplate;

        public IngredientTemplate SpecialIngredientTemplate => _specialIngredientTemplate;

        public Action<IngredientTemplate> OnSpecialIngredientChanged;
        public override void Process(Machine machine)
        {
            if (machine.InIngredients.Count == 0)
            {
                return;
            }
            
            // Sell items
            int sellPrice = 0;
            foreach(IngredientTemplate ingredientTemplate in machine.InIngredients)
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
            
            EconomyController.Instance.AddScore(sellPrice);
            machine.OnItemSell?.Invoke();
            
            // Clear the machine items
            machine.RemoveAllItems();
        }

        public override void TryGiveOutIngredient(Machine machine)
        {
            
        }

        public void SetSpecialIngredientTemplate(IngredientTemplate specialIngredient)
        {
            _specialIngredientTemplate = specialIngredient;
            OnSpecialIngredientChanged?.Invoke(specialIngredient);
		}
    }
}