using Components.Machines;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class CodexUIController : MonoBehaviour
{
    [Header("Codex base")]
    [SerializeField] private GameObject _gradient;

    [Header("Machine")]
    [SerializeField] private GameObject _contextMachineWindow;
    [SerializeField] private Image _machineImage;
    [SerializeField] private TextMeshProUGUI _machineTitle;
    [SerializeField] private TextMeshProUGUI _machineState;
    [SerializeField] private List<Image> _ingredientsIn;
    [SerializeField] private List<TextMeshProUGUI> _ingredientsInCount;
    [SerializeField] private Image _ingredientOut;
    [SerializeField] private TextMeshProUGUI _ingredientOutCount;

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
            _hoveredMachine = null;
            _contextMachineWindow.SetActive(false);
            
            return;
        }
        
        if (_hoveredMachine == machine)
        {
            return;
        }

        _hoveredMachine = machine;
        _machineImage.sprite = machine.Template.UIView;
        _machineTitle.text = machine.Template.Name;
        
        _contextMachineWindow.SetActive(true);
    }
}