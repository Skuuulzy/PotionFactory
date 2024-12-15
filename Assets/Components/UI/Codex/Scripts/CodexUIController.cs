using Components.Machines;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class CodexUIController : MonoBehaviour
{
    [Header("Codex base")]
    [SerializeField] private GameObject _gradient;

    [Header("Machine")]
    [SerializeField] private GameObject _contextMachineWindow;
    [SerializeField] private Image _machineImage;
    [SerializeField] private TextMeshProUGUI _machineTitle;
    [SerializeField] private TextMeshProUGUI _machineState;
    [SerializeField] private List<UIIngredientSlotController> _inIngredientSlots;
    [SerializeField] private UIIngredientSlotController _outIngredientSlot;

    private Machine _hoveredMachine;
    
    private void Start()
    {
        Machine.OnHovered += HandleMachineHovered;
        _contextMachineWindow.SetActive(false);
    }

    private void OnDestroy()
    {
        Machine.OnHovered -= HandleMachineHovered;
    }

    private void HandleMachineHovered(Machine machine, bool hovered)
    {
        if (!hovered)
        {
            // Reset listener of previous hovered machine
            if (_hoveredMachine != null)
            {
                _hoveredMachine.OnItemAdded -= HandleIngredientInMachineUpdated;
            }
            _hoveredMachine = null;
            _contextMachineWindow.SetActive(false);
            
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
        
        _contextMachineWindow.SetActive(true);
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