using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Components.Map
{
    public class LevelNode : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private List<LevelNode> _connectedNodes;

        private bool _isUnlocked;
        public List<LevelNode> ConnectedNodes => _connectedNodes;

        /// <summary>
        /// Initializes the node, setting up its locked or unlocked state.
        /// </summary>
        public void Initialize(bool isUnlocked)
        {
            _isUnlocked = isUnlocked;
            _button.interactable = _isUnlocked;
            _button.onClick.AddListener(OnNodeSelected);
        }

        /// <summary>
        /// Unlocks the connected nodes when this node is selected.
        /// </summary>
        private void OnNodeSelected()
        {
            // Logic to trigger level start here

            // Unlock connected nodes
            foreach (var node in _connectedNodes)
            {
                node.UnlockNode();
            }
        }

        /// <summary>
        /// Unlocks this node, making it selectable.
        /// </summary>
        private void UnlockNode()
        {
            _isUnlocked = true;
            _button.interactable = true;
        }
    }

}

