using Components.Machines;
using Components.Machines.Behaviors;
using Components.Machines.UIView;
using TMPro;
using UnityEngine;

public class RecipeContextualUIView : UIContextualComponent
{
    [SerializeField] private TMP_Text _currentRecipeText;

    private RecipeCreationBehavior _recipeCreationBehavior;
    private bool _initialized;
    
    public override void Initialize(Machine machine)
    {
        base.Initialize(machine);
        if (machine.Behavior is RecipeCreationBehavior recipeCreationBehavior)
        {
            _initialized = true;
            _recipeCreationBehavior = recipeCreationBehavior;
        }
        else
        {
            Debug.LogError($"You have added a component of type {nameof(RecipeContextualUIView)}, but your machine behaviour is not of type {nameof(RecipeCreationBehavior)}.");
        }
    }

    public override void Update()
    {
        if (!_initialized)
        {
            return;
        }

        if (_recipeCreationBehavior.ProcessingRecipe)
        {
            _currentRecipeText.text = $"{_recipeCreationBehavior.CurrentRecipe.name}, in process: {_recipeCreationBehavior.CurrentProcessTime}/{_recipeCreationBehavior.ProcessTime} ticks.";
        }
        else
        {
            _currentRecipeText.text = "None !";
        }
        
    }
}