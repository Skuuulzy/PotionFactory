using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Components.Machines.UIView
{
    public class MachineContextualUIView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _machineName;
        [SerializeField] private TMP_Text _heldIngredients;
        [SerializeField] private TMP_Text _sellPriceTxt;

        public static Action<Machine, int> OnSellMachine;
        
        private Machine _associatedMachine;
        
        public void Initialize(Machine machine)
        {
            _associatedMachine = machine;

            string cleanMachineName = machine.Controller.name.Replace("_", " ");
            _machineName.text = cleanMachineName;

            _sellPriceTxt.text = $"Sell ({machine.Template.SellPrice})";

            HandleItemAdded(false);
            _associatedMachine.OnItemAdded += HandleItemAdded;
        }

        private void OnDestroy()
        {
            _associatedMachine.OnItemAdded -= HandleItemAdded;
        }

        private void HandleItemAdded(bool _)
        {
            _heldIngredients.text = "Empty";

            if (_associatedMachine.Ingredients.Count == 0)
            {
                return;
            }
            
            var ingredientsNames = new List<string>();
            foreach (var ingredient in _associatedMachine.Ingredients)
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