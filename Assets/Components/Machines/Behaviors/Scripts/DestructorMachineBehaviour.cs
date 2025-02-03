using Components.Economy;
using Components.Ingredients;
using System;
using UnityEngine;

namespace Components.Machines.Behaviors
{
    [CreateAssetMenu(fileName = "New Machine Behaviour", menuName = "Component/Machines/Behavior/Destructor")]
    public class DestructorMachineBehaviour : MachineBehavior
    {
        public IngredientTemplate FavoriteIngredient { get; private set; }
        public Action<IngredientTemplate> OnSpecialIngredientChanged;
        
        public void SetFavoriteIngredient(IngredientTemplate specialIngredient)
        {
            FavoriteIngredient = specialIngredient;
            OnSpecialIngredientChanged?.Invoke(specialIngredient);
        }
        
        protected override void ProcessAction()
        {
            if (Machine.InIngredients.Count == 0)
            {
                return;
            }
            
            // Sell items
            int sellPrice = 0;
            foreach(IngredientTemplate ingredientTemplate in Machine.InIngredients)
            {
                if(ingredientTemplate != null && ingredientTemplate.Name == FavoriteIngredient.Name)
                {
                    sellPrice += ingredientTemplate.Price * 2;
                }
                else
                {
					sellPrice += ingredientTemplate.Price;
				}             
            }
            
            EconomyController.Instance.AddScore(sellPrice);
            Machine.OnItemSell?.Invoke();
            
            // Clear the machine items
            Machine.ClearSlot(Way.IN);
        }
    }
}