using UnityEngine;

namespace Components.Machines.ContextMenu
{
    public class MachineContextMenuController : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private MachineController _controller;

        private Transform _cameraTransform;
        private Vector3 _cameraDirection;
        private bool _initialized;

        private void Awake()
        {
            if (Camera.main == null)
            {
                Debug.LogError($"Unable to find the main camera. Billboard: {gameObject.name} will not behave as intended.");
                return;
            }

            _cameraTransform = Camera.main.transform;
            _initialized = true;
            
            Machine.OnSelected += HandleMachineSelected;

            _canvas.worldCamera = Camera.main;
            _canvas.gameObject.SetActive(false);
        }

        private void LateUpdate()
        {
            if (!_initialized || !_canvas.gameObject.activeInHierarchy)
            {
                return;
            }

            _cameraDirection = _cameraTransform.forward;
            //_cameraDirection.y = 0;

            transform.rotation = Quaternion.LookRotation(_cameraDirection);
        }
        
        private void OnDestroy()
        {
            Machine.OnSelected -= HandleMachineSelected;
        }
        
        private void HandleMachineSelected(Machine machine)
        {
            if (machine != _controller.Machine)
            {
                _canvas.gameObject.SetActive(false);
                return;
            }
            
            _canvas.gameObject.SetActive(true);
        }

        public void MoveMachine()
        {
            Debug.Log("Move Machine");
        }

        public void ConfigureMachine()
        {
            Debug.Log("Destroy Machine");
        }

        public void RetrieveMachine()
        {
            Debug.Log("Destroy Machine");
        }
    }
}