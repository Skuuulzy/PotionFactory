using Components.Economy;
using Components.Ingredients;
using System;

namespace Components.Machines.Behaviors
{
    public class MarchandMachineBehaviour : MachineBehavior
    {
        public IngredientTemplate FavoriteIngredient { get; private set; }
        
        public MarchandMachineBehaviour(Machine machine) : base(machine) { }

        public static Action<MarchandMachineBehaviour ,IngredientTemplate> OnIngredientSold;
        
        protected override void ProcessAction()
        {
            if (Machine.InIngredients.Count == 0)
            {
                Machine.OnProcess?.Invoke(Machine, false);
                return;
            }

            Machine.OnProcess?.Invoke(Machine, true);

            // Sell items
            int sellPrice = 0;
            foreach(IngredientTemplate ingredientTemplate in Machine.InIngredients)
            {
                if(FavoriteIngredient != null && ingredientTemplate != null && ingredientTemplate.Name == FavoriteIngredient.Name)
                {
                    sellPrice += ingredientTemplate.Price * 2;
                }
                else
                {
					sellPrice += ingredientTemplate.Price;
				}

                OnIngredientSold?.Invoke(this, ingredientTemplate);
            }
            
            EconomyController.Instance.AddScore(sellPrice);
            
            // Clear the machine items
            Machine.ClearSlot(Way.IN);
        }
        
        //public Action<IngredientTemplate> OnSpecialIngredientChanged;
        public void SetFavoriteIngredient(IngredientTemplate specialIngredient)
        {
            // FavoriteIngredient = specialIngredient;
            // OnSpecialIngredientChanged?.Invoke(specialIngredient);
        }
    }
}