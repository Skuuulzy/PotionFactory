using System.Collections.Generic;
using System.Linq;
using Components.Recipes;
using Database;
using UnityEngine;

namespace Components.Machines.Behaviors
{
    [CreateAssetMenu(fileName = "New Machine Behaviour", menuName = "Component/Machines/Behavior/Test")]
    public class RecipeCreationBehavior : MachineBehavior
    {
        private RecipeTemplate _currentRecipe;
        
        // TODO: Normalize behaviour with CurrentTick in MachineBehavior
        private int _currentProcessTime;
        private int _additionalRecipeProcessTime;

        public int FullProcessTime => _initialProcessTime + _additionalRecipeProcessTime - Mathf.RoundToInt((_initialProcessTime + _additionalRecipeProcessTime) * RelicEffectBonusProcessTime);
        public int CurrentProcessTime => _currentProcessTime;
        public RecipeTemplate CurrentRecipe => _currentRecipe;

        public override void Process()
        {
            // Try to find a recipe based on the machine and the items inside the machine.
            if (!ProcessingRecipe)
            {
                if (ScriptableObjectDatabase.TryFindRecipeMachine(Machine.Template, Machine.InIngredients, out var recipe))
                {
                    _currentRecipe = recipe;
                    _additionalRecipeProcessTime = Mathf.RoundToInt(Machine.Template.ProcessTime * recipe.ProcessTimeModifier);
                    
                    Debug.Log($"Machine: {Machine.Controller.name} found recipe: {_currentRecipe.name}. Start processing for {FullProcessTime} ticks.");
                }
                else
                {
                    return;
                }
            }

            ProcessingRecipe = true;
            
            // Increment the process time until we reach it.
            if (_currentProcessTime < FullProcessTime)
            {
                _currentProcessTime++;
                return;
            }

            // Is there any space left in the out slot.
            if (Machine.CanAddIngredientOfTypeInSlot(_currentRecipe.OutIngredient, Way.OUT))
            {
                // Add the ingredient to the machine out slot.
                Machine.AddIngredient(_currentRecipe.OutIngredient, Way.OUT);
                
                ProcessingRecipe = false;
                
                // Remove items used for the recipe.
                Machine.RemoveInItems(_currentRecipe.Ingredients.Keys.ToList());
                
                // Reset the recipe.
                _currentRecipe = null;
                _currentProcessTime = 0;
            }
        }

        public override void TryGiveOutIngredient()
        {
            // Try to give the item to the next machine.
            if (Machine.OutIngredients.Count <= 0)
            {
                return;
            }
            
            if (Machine.TryGetOutMachines(out List<Machine> outMachines))
            {
                var outMachine = outMachines[0];
                
                if (outMachine.CanAddIngredientOfTypeInSlot(Machine.OutIngredients.First(), Way.IN))
                {
                    var ingredientToGive = Machine.TakeOlderIngredient();
                    outMachine.AddIngredient(ingredientToGive, Way.IN);
                    
                    Debug.Log($"Machine: {Machine.Controller.name} outputting: {ingredientToGive.name} to: {outMachine.Controller.name}.");
                }
                else
                {
                    Debug.Log($"Machine: {Machine.Controller.name} cannot output to: {outMachine.Controller.name}. Because {outMachine.Controller.name} is full.");
                }
            }
            else
            {
                //Debug.Log($"Machine: {machine.Controller.name} cannot output because no out machine is connected.");
            }
        }
    }
}