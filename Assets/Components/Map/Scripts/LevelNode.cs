using Components.Bundle;
using Components.Ingredients;
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
        [SerializeField] private LevelNodeView _view;


        public List<LevelNode> ConnectedNodes => _connectedNodes;
        public IngredientsBundle IngredientsBundle => _ingredientsBundle;

		public static Action OnResetNode;
        public static Action<LevelNode> OnNodeSelected;

        /// <summary>
        /// Initializes the node, setting up its locked or unlocked state.
        /// </summary>
        public void Initialize(IngredientsBundle ingredientsBundle)
        {
            _ingredientsBundle = ingredientsBundle;
			_view.Init(ingredientsBundle);
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
            _button.interactable = true;
            UnselectNode();
		}

        public void LockNode()
        {
			_button.interactable = false;
            _button.image.color = Color.black;
		}

		public void ResetIngredientBundle()
		{
            _ingredientsBundle = null;
            _view.HandleResetLevelNode();
		}
	}

}

