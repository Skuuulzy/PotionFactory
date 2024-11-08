using Components.Bundle;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Components.Map
{
    public class LevelNode : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private List<LevelNode> _connectedNodes;
        [SerializeField] private IngredientsBundle _ingredientsBundle;
        [SerializeField] private Image _ingredientImage;

        private bool _isUnlocked;
        public List<LevelNode> ConnectedNodes => _connectedNodes;
        public IngredientsBundle IngredientsBundle => _ingredientsBundle;

        public static Action<LevelNode> OnNodeSelected;

        /// <summary>
        /// Initializes the node, setting up its locked or unlocked state.
        /// </summary>
        public void Initialize(IngredientsBundle ingredientsBundle)
        {
            _ingredientsBundle = ingredientsBundle;

            if (!ingredientsBundle.IsStartingGameBundle)
            {
                _ingredientImage.gameObject.SetActive(true);
				_ingredientImage.sprite = ingredientsBundle.IngredientsTemplatesList[0].Icon;
			}

		}

        /// <summary>
        /// Unlocks the connected nodes when this node is selected.
        /// </summary>
        public void SelectNode()
        {
            _button.image.color = Color.green;
            OnNodeSelected?.Invoke(this);
        }

        public void SelectNodeAsFirst()
		{
            _button.interactable = false;
            _button.image.color = Color.yellow;
        }

        public void UnselectNode()
		{
            _button.image.color = Color.white;
        }


        /// <summary>
        /// Unlocks this node, making it selectable.
        /// </summary>
        public void UnlockNode()
        {
            _isUnlocked = true;
            _button.interactable = true;
            UnselectNode();
		}

        public void LockNode()
        {
			_isUnlocked = false;
			_button.interactable = false;
            _button.image.color = Color.black;
		}

		internal void ResetIngredientBundle()
		{
            _ingredientsBundle = null;
			_ingredientImage.gameObject.SetActive(false);
		}
	}

}

