using System;
using System.Collections;
using System.Collections.Generic;
using Components.Ingredients;
using Components.Shop.ShopItems;
using Components.Tick;
using UnityEngine;

namespace Components.Machines
{
    public class ConveyorGridComponent : MachineGridComponent
    {
        [SerializeField] private Animator _animator;

        [Header("Ingredient")]
        [SerializeField] private IngredientController _ingredientController;
        
        private IngredientTemplate _currentIngredient;

        protected override void SetUp()
        {
            Machine.OnSlotUpdated += TranslateItem;   
        }

        private void OnDestroy()
        {
            Machine.OnSlotUpdated -= TranslateItem;
        }


        // ------------------------------------------------------------------------- ITEM -----------------------------------------------------------------------------

        private void ShowItem(bool show, IngredientTemplate ingredientToShow)
        {
            if (show)
            {
                _ingredientController.CreateRepresentationFromTemplate(ingredientToShow);
            }
            else
            {
                _ingredientController.DestroyRepresentation();
            }
        }

        // ------------------------------------------------------------------------- TRANSLATE -----------------------------------------------------------------------------

        /// <summary>
        /// Déplace l'ingrédient de manière fluide du point A au point B en fonction de l'avancement du ProcessTime.
        /// </summary>
        private void TranslateItem()
        {
            if (Machine.InIngredients.Count == 0)
            {
                _ingredientController.DestroyRepresentation();
                _currentIngredient = null;
                return;
            }

            if (_currentIngredient == null || Machine.InIngredients[0] != _currentIngredient)
            {
                ShowItem(true, Machine.InIngredients[0]);
                _animator.SetTrigger("OnItem");
            }
        }

    }
}