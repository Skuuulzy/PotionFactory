using System.Collections.Generic;
using System.Linq;
using Components.Recipes;
using Database;
using UnityEngine;

namespace Components.Machines.Behaviors
{
    [CreateAssetMenu(fileName = "New Machine Behaviour", menuName = "Machines/Behavior/Test")]
    public class RecipeCreationBehavior : MachineBehavior
    {
        private RecipeTemplate _currentRecipe;
        
        // TODO: Normalize behaviour with CurrentTick in MachineBehavior
        private int _currentProcessTime;
        private int _additionalRecipeProcessTime;

        public int FullProcessTime => InitialProcessTime + _additionalRecipeProcessTime - Mathf.RoundToInt((InitialProcessTime + _additionalRecipeProcessTime) * RelicEffectBonusProcessTime);
        public int CurrentProcessTime => _currentProcessTime;
        public RecipeTemplate CurrentRecipe => _currentRecipe;

        public override void Process(Machine machine)
        {
            // Try to find a recipe based on the machine and the items inside the machine.
            if (!ProcessingRecipe)
            {
                if (ScriptableObjectDatabase.TryFindRecipe(machine.Template, machine.InIngredients, out RecipeTemplate recipe))
                {
                    _currentRecipe = recipe;
                    _additionalRecipeProcessTime = Mathf.RoundToInt(machine.Template.ProcessTime * recipe.ProcessTimeModifier);
                    
                    Debug.Log($"Machine: {machine.Controller.name} found recipe: {_currentRecipe.name}. Start processing for {FullProcessTime} ticks.");
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

            // Is there any space left in the out slot
            if (machine.CanAddIngredientOfTypeInSlot(_currentRecipe.OutIngredient, Way.OUT))
            {
                // Add the ingredient to the machine out slot
                machine.AddIngredient(_currentRecipe.OutIngredient, Way.OUT);
            }
        }

        public override void TryGiveOutIngredient(Machine machine)
        {
            // Try to give the item to the next machine.
            if (machine.OutIngredients.Count <= 0)
            {
                return;
            }
            
            if (machine.TryGetOutMachines(out List<Machine> outMachines))
            {
                var outMachine = outMachines[0];
                
                if (outMachine.CanAddIngredientOfTypeInSlot(machine.OutIngredients.First(), Way.IN))
                {
                    var ingredientToGive = machine.TakeOlderIngredient();
                    outMachine.AddIngredient(ingredientToGive, Way.IN);
                    
                    Debug.Log($"Machine: {machine.Controller.name} outputting: {ingredientToGive.name} to: {outMachine.Controller.name}.");

                    ProcessingRecipe = false;
                    _currentRecipe = null;
                    _currentProcessTime = 0;
                    machine.ClearItems();
                }
                else
                {
                    Debug.Log($"Machine: {machine.Controller.name} cannot output to: {outMachine.Controller.name}. Because {outMachine.Controller.name} is full.");
                }
            }
            else
            {
                Debug.Log($"Machine: {machine.Controller.name} cannot output because no out machine is connected.");
            }
        }
    }
}