using UnityEngine;
using UnityEngine.UI;

namespace Components.Machines.ContextMenu
{
    public class MachineContextMenuController : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private GameObject _window;
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
            _window.SetActive(false);
        }

        private void LateUpdate()
        {
            if (!_initialized || !_window.activeInHierarchy)
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
            return;
            
            if (machine != _controller.Machine)
            {
                _window.SetActive(false);
                return;
            }

            transform.localPosition = new Vector3(0, machine.Template.ContextMenuHeight, 0);
            _configureBtn.interactable = machine.Template.CanConfigure;
            _moveBtn.interactable = machine.Template.CanMove;
            _retrieveBtn.interactable = machine.Template.CanRetrieve;
            _window.SetActive(selected);
        }

        public void ConfigureMachine()
        {
            _controller.Machine.Configure();
        }
    }
}