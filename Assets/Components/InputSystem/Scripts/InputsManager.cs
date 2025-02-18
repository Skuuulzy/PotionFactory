using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using VComponent.Tools.Singletons;

namespace VComponent.InputSystem
{
    public class InputsManager : PersistentSingleton<InputsManager>
    {
        [ShowInInspector] public bool RequestShipMove { get; private set; }
        [ShowInInspector] public bool OnInteract { get; private set; }
        [ShowInInspector] public Vector2 MoveCamera { get; private set; }
        [ShowInInspector] public bool QuickMoveCamera { get; private set; }
        [ShowInInspector] public bool ClockwiseRotationCamera { get; private set; }
        [ShowInInspector] public bool AntiClockwiseRotationCamera { get; private set; }
        [ShowInInspector] public int ZoomCamera { get; private set; }
        [ShowInInspector] public bool DragMovementCamera { get; private set; }
        [ShowInInspector] public bool DragRotationCamera { get; private set; }
        [ShowInInspector] public bool ShowLobbyInformation { get; private set; }

        private bool _isMouseOverUI;

        private void Update()
        {
            if (EventSystem.current == null)
            {
                return;
            }
            
            // This is unity recommendation, i honestly don't like this approach but it will do for now.
            _isMouseOverUI = EventSystem.current.IsPointerOverGameObject();
        }

        public void RequestShipMoveInput(InputAction.CallbackContext context)
        {
            // We want to ignore the UI.
            if (_isMouseOverUI)
            {
                return;
            }
            
            RequestShipMove = context.performed;
        }
        
        public void MoveCameraInput(InputAction.CallbackContext context)
        {
            MoveCamera = context.ReadValue<Vector2>();
        }

        public void QuickMoveCameraInput(InputAction.CallbackContext context)
        {
            QuickMoveCamera = context.performed;
        }
        
        public void ClockwiseRotationCameraInput(InputAction.CallbackContext context)
        {
            ClockwiseRotationCamera = context.performed;
        }
        
        public void AntiClockwiseRotationCameraInput(InputAction.CallbackContext context)
        {
            AntiClockwiseRotationCamera = context.performed;
        }

        public void ScrollInput(InputAction.CallbackContext context)
        {
            // We want to ignore the UI.
            if (_isMouseOverUI)
            {
                return;
            }
            
            var scrollValue = context.ReadValue<float>();
            if (scrollValue > 0)
            {
                ZoomCamera = 1;
            }
            else if (scrollValue < 0)
            {
                ZoomCamera = -1;
            }
            else
            {
                ZoomCamera = 0;
            }
        }
        
        public void DragMovementCameraInput(InputAction.CallbackContext context)
        {
            // We want to ignore the UI.
            if (_isMouseOverUI)
            {
                return;
            }
            
            //Debug.Log($"Start drag: {context.started}, DragMovement: {context.performed}");
            DragMovementCamera = context.performed;
        }
        
        public void DragRotationCameraInput(InputAction.CallbackContext context)
        {
            // We want to ignore the UI.
            if (_isMouseOverUI)
            {
                return;
            }
            
            //Debug.Log($"Start drag: {context.started}, DragMovement: {context.performed}");
            DragRotationCamera = context.performed;
        }

        public void ShowLobbyInformationInput(InputAction.CallbackContext context)
        {
            ShowLobbyInformation = context.performed;
        }

        public void Interact(InputAction.CallbackContext context)
        {
			OnInteract = context.performed;
		}
    }
}