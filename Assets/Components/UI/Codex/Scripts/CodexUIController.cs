using Components.Machines;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Components.Machines.Behaviors;

public class CodexUIController : MonoBehaviour
{
    [SerializeField] private GameObject _window;
    [Header("Machine")]
    [SerializeField] private GameObject _contextMachineWindow;
    [SerializeField] private Image _machineImage;
    [SerializeField] private TextMeshProUGUI _machineTitle;
    [SerializeField] private TextMeshProUGUI _machineState;
    [SerializeField] private List<UIIngredientSlotController> _inIngredientSlots;
    [SerializeField] private UIIngredientSlotController _outIngredientSlot;
    [SerializeField] private Slider _processTimeSlider;
    [SerializeField] private TMP_Text _processTimeText;

    [Header("Generic")]
    [SerializeField] private GameObject _contextGenericWindow;
    [SerializeField] private Image _genericMachineImage;
    [SerializeField] private TextMeshProUGUI _genericMachineTitle;
    [SerializeField] private TextMeshProUGUI _genericMachineState;
    [SerializeField] private TextMeshProUGUI _genericMachineLore;
    
    private Machine _hoveredMachine;
    private bool _recipeMachine;
    
    private void Start()
    {
        Machine.OnHovered += HandleMachineHovered;
        _window.SetActive(false);
        
        _processTimeSlider.value = 0;
        _processTimeText.text = "0/0 ticks";
    }

    private void Update()
    {
        if (!_recipeMachine)
        {
            return;
        }

        if (_hoveredMachine.Behavior is not RecipeCreationBehavior recipeCreationBehavior)
        {
            return;
        }

        if (!recipeCreationBehavior.ProcessingRecipe)
        {
            _processTimeSlider.value = 0;
            _processTimeText.text = "0/0 ticks";
            
            return;
        }

        // Temporary solution to show what recipe is currently processed.
        if (_outIngredientSlot.ShowEmpty)
        {
            _outIngredientSlot.SetIngredientSlot(recipeCreationBehavior.CurrentRecipe.OutIngredient.Icon,0,_hoveredMachine.Template.IngredientsPerSlotCount);
        }
        
        _processTimeSlider.value = recipeCreationBehavior.CurrentProcessTime / (float)recipeCreationBehavior.ProcessTime;
        _processTimeText.text = $"{recipeCreationBehavior.CurrentProcessTime}/{recipeCreationBehavior.ProcessTime} ticks";
    }

    private void OnDestroy()
    {
        Machine.OnHovered -= HandleMachineHovered;
    }

    private void HandleMachineHovered(Machine machine, bool hovered)
    {
        _window.SetActive(hovered);
        _recipeMachine = false;
        
        // Special case for other machine. TODO: Handle it directly in the template to have generic method here.
        if (machine.Template.Type == MachineType.DISPENSER || machine.Template.Type == MachineType.MARCHAND)
        {
            _contextGenericWindow.SetActive(hovered);
            _contextMachineWindow.SetActive(!hovered);
            
            _genericMachineTitle.text = machine.Template.Name;
            _genericMachineState.text = machine.Template.UIGameplayDescription;
            _genericMachineLore.text = machine.Template.UILoreDescription;
            
            switch (machine.Behavior)
            {
                case DestructorMachineBehaviour destructorMachineBehaviour:
                    _genericMachineTitle.text += $" ({destructorMachineBehaviour.FavoriteIngredient.Name.ToLower()})";
                    _genericMachineImage.sprite = destructorMachineBehaviour.FavoriteIngredient.Icon;
                    break;
                case ExtractorMachineBehaviour extractorMachineBehaviour:
                    if (extractorMachineBehaviour.IngredientToExtract)
                    {
                        _genericMachineTitle.text += $" ({extractorMachineBehaviour.IngredientToExtract.Name.ToLower()})";
                        _genericMachineImage.sprite = extractorMachineBehaviour.IngredientToExtract.Icon;
                    }
                    break;
            }

            return;
        }
        
        _contextMachineWindow.SetActive(hovered);
        _contextGenericWindow.SetActive(!hovered);
        
        if (!hovered)
        {
            // Reset listener of previous hovered machine
            if (_hoveredMachine != null)
            {
                _hoveredMachine.OnItemAdded -= HandleIngredientInMachineUpdated;
            }
            _hoveredMachine = null;
            
            return;
        }
        
        if (_hoveredMachine == machine)
        {
            return;
        }

        // Reset listener of previous hovered machine
        if (_hoveredMachine != null)
        {
            _hoveredMachine.OnItemAdded -= HandleIngredientInMachineUpdated;
        }
        
        _hoveredMachine = machine;
        _machineImage.sprite = machine.Template.UIView;
        _machineTitle.text = machine.Template.Name;
        
        UpdateIngredientInSlots(machine);
        _hoveredMachine.OnItemAdded += HandleIngredientInMachineUpdated;

        if (machine.Behavior is RecipeCreationBehavior)
        {
            _recipeMachine = true;
        }
    }

    private void HandleIngredientInMachineUpdated(bool _)
    {
        UpdateIngredientInSlots(_hoveredMachine);
    }

    private void UpdateIngredientInSlots(Machine machine)
    {
        // Setup in ingredients slots
        var inIngredientCountByType = machine.GroupedInIngredients;
        
        for (var i = 0; i < _inIngredientSlots.Count; i++)
        {
            var ingredientSlot = _inIngredientSlots[i];

            if (i > machine.Template.InSlotIngredientCount - 1)
            {
                ingredientSlot.gameObject.SetActive(false);
                continue;
            }
            
            ingredientSlot.gameObject.SetActive(true);
            if (i < inIngredientCountByType.Count)
            {
                var ingredientData = inIngredientCountByType.ElementAt(i);
                ingredientSlot.SetIngredientSlot(ingredientData.Key.Icon, ingredientData.Value, machine.Template.IngredientsPerSlotCount);
                continue;
            }

            ingredientSlot.SetEmpty(machine.Template.IngredientsPerSlotCount);
        }

        
        if (machine.GroupedOutIngredients.Count > 0)
        {
            // The out ingredients should only have one category of item.
            var ingredientData = machine.GroupedOutIngredients.ElementAt(0);
            _outIngredientSlot.SetIngredientSlot(ingredientData.Key.Icon, ingredientData.Value, machine.Template.IngredientsPerSlotCount);
        }
        else
        {
            _outIngredientSlot.SetEmpty(machine.Template.IngredientsPerSlotCount);
        }
    }
}