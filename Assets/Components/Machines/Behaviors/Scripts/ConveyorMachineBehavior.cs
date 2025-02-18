using Components.Ingredients;
using UnityEngine;

namespace Components.Machines.Behaviors
{
    public class ConveyorMachineBehavior : MachineBehavior
    {
        public ConveyorMachineBehavior(Machine machine) : base(machine) { }

        /// The conveyor will not do the base action if the next machine cannot take is ingredient.
        /// This prevents that the conveyor store 2 ingredients.
        protected override void ProcessAction()
        {
            if (Machine.InIngredients.Count <= 0)
            {
                Machine.OnProcess(Machine, false);
                return;
            }
            
            var machineToOutput = OutputMachine();
            if (machineToOutput == null)
            {
                return;
            }
        
            var ingredientToMove = Machine.InIngredients[0];
            if (!machineToOutput.CanTakeIngredientInSlot(ingredientToMove, Way.IN))
            {
                return;
            }
            
            base.ProcessAction();
        }
    }
}