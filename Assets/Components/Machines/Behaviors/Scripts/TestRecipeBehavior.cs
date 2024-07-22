using Components.Recipes;
using UnityEngine;

namespace Components.Machines.Behaviors
{
    [CreateAssetMenu(fileName = "New Machine Behaviour", menuName = "Machines/Behavior/Test")]
    public class TestRecipeBehavior : MachineBehavior
    {
        private RecipeTemplate _currentRecipe;
        private int _currentProcessTime;
        
        public override void Process(Machine machine)
        {
            // Try to find a recipe based on the machine and the items inside the machine.
            if (!_currentRecipe)
            {
                if (_recipeManager.TryFindRecipe(machine.Template, machine.Items, out RecipeTemplate recipe))
                {
                    _currentRecipe = recipe;
                }
                else
                {
                    return;
                }
            }
            
            // Increment the process time until we reach it.
            if (_currentProcessTime < _processTime)
            {
                _currentProcessTime++;
                return;
            }
            
            // Try to give the item to the next machine.
            if (machine.TryGetOutMachine(out Machine outMachine))
            {
                if (outMachine.TryGiveItemItem(_currentRecipe.OutIngredient))
                {
                    Debug.Log($"Machine: {machine.Controller.name} outputting: {_currentRecipe.OutIngredient.name} to: {outMachine.Controller.name}.");
                    
                    _currentRecipe = null;
                    _currentProcessTime = 0;
                    machine.ClearItems();
                }
            }
        }
    }
}