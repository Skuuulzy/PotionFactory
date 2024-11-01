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

        private bool _isUnlocked;
        public List<LevelNode> ConnectedNodes => _connectedNodes;
        public IngredientsBundle IngredientsBundle => _ingredientsBundle;

        public static Action<LevelNode> OnNodeSelected;

        /// <summary>
        /// Initializes the node, setting up its locked or unlocked state.
        /// </summary>
        public void Initialize(bool isUnlocked, IngredientsBundle ingredientsBundle)
        {
            _isUnlocked = isUnlocked;
            _button.interactable = _isUnlocked;
            _ingredientsBundle = ingredientsBundle;
        }

        /// <summary>
        /// Unlocks the connected nodes when this node is selected.
        /// </summary>
        public void SelectNode()
        {
            // Unlock connected nodes
            foreach (var node in _connectedNodes)
            {
                node.UnlockNode();
            }

            OnNodeSelected?.Invoke(this);
        }

        /// <summary>
        /// Unlocks this node, making it selectable.
        /// </summary>
        private void UnlockNode()
        {
            _isUnlocked = true;
            _button.interactable = true;
        }

        public void LockNode()
        {
			_isUnlocked = false;
			_button.interactable = false;
		}


    }

}

