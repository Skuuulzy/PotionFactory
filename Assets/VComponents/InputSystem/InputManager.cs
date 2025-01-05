using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using VComponent.Tools.Singletons;

namespace VComponents.InputSystem
{
    public abstract class InputManager : Singleton<InputManager>
    {
        [Header("Input Components")]
        [SerializeField] protected PlayerInput _playerInput;
        
        public string GetCurrentControlScheme() => _playerInput.currentControlScheme;
        public bool IsUsingGamepad() => _playerInput.currentControlScheme == "Gamepad";
        public bool IsUsingKeyboard() => _playerInput.currentControlScheme == "Keyboard&Mouse";

        private const string BINDING_KEY_ID = "BINDING_";
        
        protected override void InitializeSingleton()
        {
            base.InitializeSingleton();
            
            if (_playerInput == null)
            {
                _playerInput = GetComponent<PlayerInput>();
            }
            else
            {
                Debug.LogError($"[INPUT MANAGER] Unable to found a player input component. NO INPUT AVAILABLE.");
                Destroy(this);
            }
        }
        
        // ----------------------------------------- INPUT ACTIONS MANAGEMENT -------------------------------------------------
        protected void EnableAction(string actionName)
        {
            var action = _playerInput.actions[actionName];
            if (action != null && !action.enabled)
            {
                action.Enable();
            }
            else
            {
                Debug.LogError($"[INPUT MANAGER] Action '{actionName}' not found. Cannot enable.");
            }
        }

        protected void DisableAction(string actionName)
        {
            var action = _playerInput.actions[actionName];
            if (action != null && action.enabled)
            {
                action.Disable();
            }
            else
            {
                Debug.LogError($"[INPUT MANAGER] Action '{actionName}' not found. Cannot disable.");
            }
        }

        protected bool IsActionPressed(string actionName)
        {
            var action = _playerInput.actions[actionName];
            if (action != null)
            {
                return action.IsPressed();
            }

            Debug.LogError($"[INPUT MANAGER] Action '{actionName}' not found. Cannot determine if pressed.");
            return false;
        }
        
        protected bool WasActionPerformed(string actionName)
        {
            var action = _playerInput.actions[actionName];
            if (action != null)
            {
                return action.triggered;
            }

            Debug.LogError($"[INPUT MANAGER] Action '{actionName}' not found. Cannot determine if pressed.");
            return false;
        }
        
        /// <summary>
        /// Reads the value of an input action as a generic type.
        /// Supports Vector2, float, bool, etc., with type validation.
        /// </summary>
        /// <typeparam name="T">The expected type of the input value.</typeparam>
        /// <param name="actionName">The name of the input action.</param>
        /// <returns>The input value if valid; otherwise, the default value of T.</returns>
        protected T GetActionValue<T>(string actionName) where T : struct
        {
            var action = _playerInput.actions[actionName];

            if (action == null)
            {
                Debug.LogError($"[INPUT MANAGER] Action '{actionName}' not found. Cannot read value.");
                return default;
            }

            // Ensure the expected type matches the action's actual value type
            Type expectedType = typeof(T);
            Type actualType = action.expectedControlType switch
            {
                "Vector2" => typeof(Vector2),
                "Axis" or "float" => typeof(float),
                "Button" or "bool" => typeof(bool),
                _ => null
            };

            if (actualType == null)
            {
                Debug.LogError($"[INPUT MANAGER] Unknown control type '{action.expectedControlType}' for action '{actionName}'.");
            }
            else if (expectedType != actualType)
            {
                Debug.LogError($"[INPUT MANAGER] Type mismatch for action '{actionName}'. Expected '{expectedType}', but action provides '{actualType}'.");
                return default;
            }

            return action.ReadValue<T>();
        }
        
        // ----------------------------------------- EVENT-BASED INPUT HANDLING -------------------------------------------------
        protected void SubscribeToAction(string actionName, Action<InputAction.CallbackContext> callback)
        {
            var action = _playerInput.actions[actionName];
            if (action != null)
            {
                action.performed += callback;
            }
            else
            {
                Debug.LogError($"[INPUT MANAGER] Action '{actionName}' not found. Cannot subscribe to it.");
            }
        }

        protected void UnsubscribeFromAction(string actionName, Action<InputAction.CallbackContext> callback)
        {
            var action = _playerInput.actions[actionName];
            if (action != null)
            {
                action.performed -= callback;
            }
            else
            {
                Debug.LogError($"[INPUT MANAGER] Action '{actionName}' not found. Cannot unsubscribe from it.");
            }
        }
        
        // ----------------------------------------- CONTROLLER MANAGEMENT -------------------------------------------------
        public void SwitchControlScheme(string controlScheme)
        {
            if (_playerInput != null)
            {
                _playerInput.defaultControlScheme = controlScheme;
            }
            else
            {
                Debug.LogError($"[INPUT MANAGER] Control Scheme '{controlScheme}' not found. Cannot switch to it.");
            }
        }
        
        // ----------------------------------------- ACTION MAP -------------------------------------------------
        /// <summary>
        /// Enables a specific action map, optionally disable others.
        /// </summary>
        public void EnableActionMap(string actionMapName, bool disableOther = false)
        {
            var actionMap = _playerInput.actions.FindActionMap(actionMapName);
            if (actionMap != null && !actionMap.enabled)
            {
                actionMap.Enable();
            }
            else
            {
                Debug.LogError($"[INPUT MANAGER] Action Map '{actionMapName}' not found. Cannot enable it.");
            }

            if (disableOther)
            {
                DisableAllActionMapsExcept(actionMapName);
            }
        }

        /// <summary>
        /// Disables a specific action map.
        /// </summary>
        public void DisableActionMap(string actionMapName)
        {
            var actionMap = _playerInput.actions.FindActionMap(actionMapName);
            if (actionMap != null && actionMap.enabled)
            {
                actionMap.Disable();
            }
            else
            {
                Debug.LogError($"[INPUT MANAGER] Action Map '{actionMapName}' not found. Cannot disable it.");
            }
        }

        /// <summary>
        /// Disables all action maps except the specified one.
        /// Useful when switching contexts (e.g., UI ↔ Gameplay).
        /// </summary>
        public void DisableAllActionMapsExcept(string actionMapToKeep)
        {
            foreach (var actionMap in _playerInput.actions.actionMaps)
            {
                if (actionMap.name != actionMapToKeep)
                {
                    actionMap.Disable();
                }
            }
        }

        /// Checks if an action map is currently enabled.
        public bool IsActionMapEnabled(string actionMapName)
        {
            var actionMap = _playerInput.actions.FindActionMap(actionMapName);
            if (actionMap != null)
            {
                return actionMap.enabled;
            }

            Debug.LogError($"[INPUT MANAGER] Action Map '{actionMapName}' not found. Cannot check if enabled.");
            return false;
        }

        /// Gets a list of all currently enabled action maps.
        public List<string> GetEnabledActionMaps()
        {
            List<string> activeMaps = new List<string>();

            foreach (var actionMap in _playerInput.actions.actionMaps)
            {
                if (actionMap.enabled)
                {
                    activeMaps.Add(actionMap.name);
                }
            }

            return activeMaps;
        }

        // ----------------------------------------- INPUT REBINDING SYSTEM -------------------------------------------------
        /// Starts rebinding an input action and triggers a callback when complete.
        public void StartRebinding(string actionName, int bindingIndex, Action<string> onComplete)
        {
            var action = _playerInput.actions[actionName];
            if (action == null)
            {
                Debug.LogWarning($"[INPUT MANAGER] Action '{actionName}' not found. Cannot rebind.");
                return;
            }

            if (bindingIndex < 0 || bindingIndex >= action.bindings.Count)
            {
                Debug.LogWarning($"Invalid binding index for '{actionName}'. Cannot rebind.");
                return;
            }

            action.Disable();

            action.PerformInteractiveRebinding(bindingIndex)
                .WithControlsExcluding("Mouse") // Optional: Exclude mouse if needed
                .OnComplete(operation =>
                {
                    string newBinding = action.bindings[bindingIndex].effectivePath;
                    onComplete?.Invoke(newBinding);

                    operation.Dispose();
                    action.Enable();

                    SaveBinding(actionName);
                }).Start();
        }

        /// Saves an action's current binding to PlayerPrefs.
        public void SaveBinding(string actionName)
        {
            var action = _playerInput.actions[actionName];

            if (action == null)
            {
                Debug.LogWarning($"[INPUT MANAGER] Action '{actionName}' not found. Cannot save binding.");
                return;
            }

            var bindingJson = action.SaveBindingOverridesAsJson();
            PlayerPrefs.SetString($"{BINDING_KEY_ID}{actionName}", bindingJson);
            PlayerPrefs.Save();
        }

        /// Loads saved bindings from PlayerPrefs.
        public void LoadBindings()
        {
            foreach (var action in _playerInput.actions)
            {
                var key = $"{BINDING_KEY_ID}{action.name}";
                if (PlayerPrefs.HasKey(key))
                {
                    action.LoadBindingOverridesFromJson(PlayerPrefs.GetString(key));
                }
            }
        }

        /// <summary>
        /// Resets an action to its default binding.
        /// </summary>
        public void ResetBinding(string actionName)
        {
            var action = _playerInput.actions[actionName];

            if (action == null)
            {
                Debug.LogWarning($"[INPUT MANAGER] Action '{actionName}' not found. Cannot reset binding.");
                return;
            }

            action.RemoveAllBindingOverrides();
            PlayerPrefs.DeleteKey($"{BINDING_KEY_ID}{actionName}");
        }

        /// Resets all bindings to defaults.
        public void ResetAllBindings()
        {
            foreach (var action in _playerInput.actions)
            {
                action.RemoveAllBindingOverrides();
                PlayerPrefs.DeleteKey($"{BINDING_KEY_ID}{action.name}");
            }
        }
    }
}
