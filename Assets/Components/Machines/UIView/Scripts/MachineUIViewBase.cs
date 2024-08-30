using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Components.Machines.UIView
{
    public class MachineUIViewBase : MonoBehaviour
    {
        [SerializeField] private TMP_Text _machineName;
        [SerializeField] private TMP_Text _heldIngredients;
        
        protected Machine AssociatedMachine;
        
        public virtual void Initialize(Machine machine)
        {
            AssociatedMachine = machine;
            _machineName.text = machine.Controller.name;

            HandleItemAdded(false);
            AssociatedMachine.OnItemAdded += HandleItemAdded;
        }

        private void OnDestroy()
        {
            AssociatedMachine.OnItemAdded -= HandleItemAdded;
        }

        private void HandleItemAdded(bool _)
        {
            _heldIngredients.text = "Empty";

            if (AssociatedMachine.Ingredients.Count == 0)
            {
                return;
            }

            var ingredientsNames = new List<string>();
            foreach (var ingredient in AssociatedMachine.Ingredients)
            {
                ingredientsNames.Add(ingredient.Name);
            }
            
            string ingredientsInMachine = string.Join("\n", ingredientsNames);
            _heldIngredients.text = ingredientsInMachine;
        }
    }
}