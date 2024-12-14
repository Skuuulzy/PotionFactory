using Components.Machines;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class CodexController : MonoBehaviour
{
    [Header("Codex base")]
    [SerializeField] private GameObject _gradient;

    [Header("Machine")]
    [SerializeField] private GameObject _contentMachine;
    [SerializeField] private Image _machineImage;
    [SerializeField] private TextMeshProUGUI _machineTitle;
    [SerializeField] private TextMeshProUGUI _machineState;
    [SerializeField] private List<Image> _ingrédientsIN;
    [SerializeField] private List<TextMeshProUGUI> _ingrédientsINNumber;
    [SerializeField] private Image _ingrédientOUT;
    [SerializeField] private TextMeshProUGUI _ingrédientOUTNumber;

    private void Awake()
    {
        Machine.OnSelected += HandleMachineHovered;
    }

    private void OnDestroy()
    {
        Machine.OnSelected -= HandleMachineHovered;
    }

    public void HandleMachineHovered(Machine machine)
    {
        _machineImage.sprite = machine.Template.UIView;
        _machineTitle.text = machine.Template.Name;

        for(int i = 0; i < machine.Ingredients.Count; i++) 
        {
            if(i < machine.Ingredients.Count -1)
            {
                //_ingrédientsIN[i].sprite = machine.Ingredients[i].Icon;
                //_ingrédientsINNumber[i].text = $"{machine.Ingredients[i].}/5";
                //si pas d'ingrédient dans le slot, pas de sprite + texte = "-"
            }
            else
            {

            }
        }

        //Temporaire en attendant le hover et l'animation
        _gradient.SetActive(true);
        _contentMachine.SetActive(true);
    }
}
