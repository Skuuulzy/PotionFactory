using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace VComponents.InputSystem.Examples
{
    public class ExampleInputManager : InputManager
    {
        private const string MAIN_MENU_ACTION_MAP_NAME = "MainMenu";
        private const string LEVEL_ACTION_MAP_NAME = "Level";
        
        private const string CONFIRM_ACTION_NAME = "Confirm";
        
        private const string JUMP_ACTION_NAME = "Jump";
        private const string MOVE_ACTION_NAME = "Move";
        
        public event Action<InputAction.CallbackContext> OnConfirm;
        
        public Vector2 MovementVector { get; private set; }
        public event Action<InputAction.CallbackContext> OnJump;
        
        public void SwitchToMainMenuActionMap()
        {
            EnableActionMap(MAIN_MENU_ACTION_MAP_NAME, true);
            BindMainMenuActions();
        }
        
        public void SwitchToLevelActionMap()
        {
            EnableActionMap(LEVEL_ACTION_MAP_NAME, true);
        }

        // ---------------------------------- MAIN MENU ------------------------------------
        private void BindMainMenuActions()
        {
            SubscribeToAction(CONFIRM_ACTION_NAME, OnConfirm);
        }

        private void UnbindMainMenuActions()
        {
            UnsubscribeFromAction(CONFIRM_ACTION_NAME, OnConfirm);
        }
        
        // ----------------------------------- LEVEL -----------------------------------------
        private void BindLevelActions()
        {
            SubscribeToAction(JUMP_ACTION_NAME, OnJump);
        }

        private void UnbindLevelActions()
        {
            UnsubscribeFromAction(JUMP_ACTION_NAME, OnJump);
        }

        private void Update()
        {
            if (!IsActionMapEnabled(LEVEL_ACTION_MAP_NAME))
            {
                return;
            }

            MovementVector = GetActionValue<Vector2>(MOVE_ACTION_NAME);
        }

        // Additional help:
        // If the class get too dense:
        // - Consider using partial classes that separated Action maps: ExampleInputManager.MainMenu.cs ExampleInputManager.Level.
        // - Split input manager by scene, you can have a specific input manager singleton in needed scenes.
        //   Be careful if you do this do not check persistent on the singleton inspector.
    }
}