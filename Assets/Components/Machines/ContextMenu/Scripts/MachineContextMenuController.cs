using UnityEngine;
using UnityEngine.UI;

namespace Components.Machines.ContextMenu
{
    public class MachineContextMenuController : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private MachineController _controller;

        [SerializeField] private Button _configureBtn;
        [SerializeField] private Button _moveBtn;
        [SerializeField] private Button _retrieveBtn;
        
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

            transform.rotation = Quaternion.LookRotation(_cameraDirection);
        }
        
        private void OnDestroy()
        {
            Machine.OnSelected -= HandleMachineSelected;
        }
        
        private void HandleMachineSelected(Machine machine, bool selected)
        {
            if (machine != _controller.Machine)
            {
                _canvas.gameObject.SetActive(false);
                return;
            }

            transform.localPosition = new Vector3(0, machine.Template.ContextMenuHeight, 0);
            _configureBtn.interactable = machine.Template.CanConfigure;
            _moveBtn.interactable = machine.Template.CanMove;
            _retrieveBtn.interactable = machine.Template.CanRetrieve;
            _canvas.gameObject.SetActive(selected);
        }

        public void MoveMachine()
        {
            _controller.Move();
            _canvas.gameObject.SetActive(false);
        }

        public void ConfigureMachine()
        {
            _controller.Configure();
        }

        public void RetrieveMachine()
        {
            _controller.Retrieve();
            _canvas.gameObject.SetActive(false);
        }
    }
}