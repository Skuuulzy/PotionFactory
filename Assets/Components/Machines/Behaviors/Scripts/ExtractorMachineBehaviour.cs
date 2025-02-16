using Components.Ingredients;

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
        
		protected override void ProcessAction()
        {
            // Extract ingredients if any room left
            if (Machine.CanTakeIngredientInSlot(IngredientToExtract, Way.IN))
            {
                Machine.AddIngredient(IngredientToExtract, Way.IN);
            }
            
            // Apply the base transfer ingredient sub process
            base.ProcessAction();
        }
    }
}