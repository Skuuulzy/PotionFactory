using System;
using UnityEngine;
using UnityEngine.InputSystem;
using VComponents.InputSystem;

namespace VComponent.InputSystem
{
    public class InputManager: BaseInputManager<InputManager>
    {
        private const string MOVE_ACTION_NAME = "Move";
        public event Action<Vector2> OnMove;

        private void BindLevelActions()
        {
            SubscribeToAction(MOVE_ACTION_NAME, OnMoveWrapper);
        }

        private void OnMoveWrapper(InputAction.CallbackContext callback)
        {
            OnMove?.Invoke(callback.ReadValue<Vector2>());
        }
    }
}