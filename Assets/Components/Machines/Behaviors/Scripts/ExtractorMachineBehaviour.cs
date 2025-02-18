using Components.Ingredients;
using UnityEngine;

namespace Components.Machines.Behaviors
{
    public class ExtractorMachineBehaviour : MachineBehavior
    {
        public ExtractorMachineBehaviour(Machine machine) : base(machine) { }

        public IngredientTemplate IngredientToExtract { get; private set; }

        public void SetExtractedIngredient(IngredientTemplate ingredientTemplate)
        {
            IngredientToExtract = ingredientTemplate;
        }

        protected override void PreProcess()
        {
            AdditionalProcessTime = Mathf.RoundToInt(IngredientToExtract.ExecutionTimeModifier);
            base.PreProcess();
        }

        protected override void ProcessAction()
        {
            // Extract ingredients if any room left 
            if (Machine.CanTakeIngredientInSlot(IngredientToExtract, Way.IN))
            {
                Machine.AddIngredient(IngredientToExtract, Way.IN);
            }

            // Reset the recipe. 
            AdditionalProcessTime -= Mathf.RoundToInt(IngredientToExtract.ExecutionTimeModifier);
            // Apply the base transfer ingredient sub process 
            base.ProcessAction();
        }
    }
}