using Components.Recipes;
using Database;
using UnityEngine;

namespace Components.Machines.Behaviors
{
    [CreateAssetMenu(fileName = "New Machine Behaviour", menuName = "Machines/Behavior/Test")]
    public class RecipeCreationBehavior : MachineBehavior
    {
        private RecipeTemplate _currentRecipe;
        private int _currentProcessTime;
        
        public override void Process(Machine machine)
        {
            // Try to find a recipe based on the machine and the items inside the machine.
            if (!ProcessingRecipe)
            {
                if (ScriptableObjectDatabase.TryFindRecipe(machine.Template, machine.Ingredients, out RecipeTemplate recipe))
                {
                    _currentRecipe = recipe;
                    Debug.Log($"Machine: {machine.Controller.name} found recipe: {_currentRecipe.name}. Start processing for {_processTime} ticks.");
                }
                else
                {
                    return;
                }
            }

            ProcessingRecipe = true;
            
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

                    ProcessingRecipe = false;
                    _currentRecipe = null;
                    _currentProcessTime = 0;
                    machine.ClearItems();
                }
                else
                {
                    Debug.Log($"Machine: {machine.Controller.name} cannot output: {_currentRecipe.OutIngredient.name} to: {outMachine.Controller.name}. Because {outMachine.Controller.name} is either full or processing a recipe");
                }
            }
            else
            {
                Debug.Log($"Machine: {machine.Controller.name} cannot output: {_currentRecipe.OutIngredient.name} because no out machine is connected.");
            }
        }
    }
}