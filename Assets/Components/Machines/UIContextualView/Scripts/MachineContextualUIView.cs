using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Components.Machines.UIView
{
    public class MachineContextualUIView : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private TMP_Text _machineName;
        [SerializeField] private TMP_Text _heldIngredients;
        [SerializeField] private TMP_Text _sellPriceTxt;
        [SerializeField] private Button _sellButton;
        [SerializeField] private Transform _componentsHolder;
        
        public static Action<Machine, int> OnSellMachine;
        
        private Machine _associatedMachine;
        private List<UIContextualComponent> _contextualComponents;
        
        // --------------------------------------------------------- UI INITIALIZATION ---------------------------------------------------------
        public void Initialize(Machine machine)
        {
            _associatedMachine = machine;

            string cleanMachineName = machine.Controller.name.Replace("_", " ");
            _machineName.text = cleanMachineName;

            if (machine.Template.CanRetrieve)
            {
                _sellButton.interactable = false;
                _sellPriceTxt.text = $"Cannot be sell";
            }
            else
            {
                _sellButton.interactable = true;
                _sellPriceTxt.text = $"Sell ({machine.Template.SellPrice})";
            }

            HandleItemAdded();
            _associatedMachine.OnSlotUpdated += HandleItemAdded;
        }

        private void OnDestroy()
        {
            _associatedMachine.OnSlotUpdated -= HandleItemAdded;
        }

        public void AddComponents(List<UIContextualComponent> contextualComponents)
        {
            ClearComponents();
            
            _contextualComponents = new List<UIContextualComponent>();
            
            foreach (var contextualComponent in contextualComponents)
            {
                var component = Instantiate(contextualComponent, _componentsHolder);
                component.Initialize(_associatedMachine);
                
                _contextualComponents.Add(component);
            }
        }

        private void ClearComponents()
        {
            if (_contextualComponents == null)
            {
                return;
            }

            foreach (var contextualComponent in _contextualComponents)
            {
                Destroy(contextualComponent.gameObject);
            }
            
            _contextualComponents.Clear();
        }
        
        // ------------------------------------------------------- BASE MACHINE CONTEXTUAL ------------------------------------------------------
        private void HandleItemAdded()
        {
            _heldIngredients.text = "Empty";

            if (_associatedMachine.InIngredients.Count == 0)
            {
                return;
            }
            
            var ingredientsNames = new List<string>();
            foreach (var ingredient in _associatedMachine.InIngredients)
            {
                ingredientsNames.Add(ingredient.Name);
            }

            // Group ingredients by type and quantity.
            var groupedIngredients = ingredientsNames
                .GroupBy(item => item)
                .Select(group => $"{group.Key} (x{group.Count()})");

            string result = string.Join(", ", groupedIngredients);
            
            _heldIngredients.text = result;
        }

        public void SellMachine()
        {
            OnSellMachine?.Invoke(_associatedMachine, _associatedMachine.Template.SellPrice);
            Destroy(gameObject);
        }
    }
}