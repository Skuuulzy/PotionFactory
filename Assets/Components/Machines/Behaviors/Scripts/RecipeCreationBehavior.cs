using Components.Recipes;
using Database;
using UnityEngine;

namespace Components.Machines.Behaviors
{
    public class RecipeCreationBehavior : MachineBehavior
    {
        public RecipeTemplate CurrentRecipe { get; private set; }
        public bool ProcessingRecipe => CurrentRecipe != null;

        public RecipeCreationBehavior(Machine machine) : base(machine) { }
        
        protected override void PreProcess()
        {
            // Try to find a recipe based on the machine and the items inside the machine.
            if (ProcessingRecipe)
            {
                return;
            }

            if (!ScriptableObjectDatabase.TryFindRecipeMachine(Machine.Template, Machine.InIngredients, out var recipe))
            {
                return;
            }
            
            CurrentRecipe = recipe;
            AdditionalProcessTime += Mathf.RoundToInt(Machine.Template.ProcessTime * recipe.ProcessTimeModifier);
            
            // Remove items used for the recipe.
            foreach (var recipeIngredient in CurrentRecipe.Ingredients)
            {
                for (int i = 0; i < recipeIngredient.Value; i++)
                {
                    Machine.RemoveIngredient(recipeIngredient.Key,Way.IN);
                }
            }
            
            Debug.Log($"Machine: {Machine.Controller.name} found recipe: {CurrentRecipe.name}. Start processing for {ProcessTime} ticks.");
        }

        protected override void ProcessAction()
        {
            if (!ProcessingRecipe)
            {
                return;
            }
            
            // Is there any space left in the out slot.
            if (!Machine.CanTakeIngredientInSlot(CurrentRecipe.OutIngredient, Way.OUT))
            {
                return;
            }
            
            // Add the ingredient to the machine out slot.
            Machine.AddIngredient(CurrentRecipe.OutIngredient, Way.OUT);
                
            // Reset the recipe.
            AdditionalProcessTime -= Mathf.RoundToInt(Machine.Template.ProcessTime * CurrentRecipe.ProcessTimeModifier);
            CurrentRecipe = null;
        }
    }
}