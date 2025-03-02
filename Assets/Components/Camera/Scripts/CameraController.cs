using SoWorkflow.SharedValues;
using UnityEngine;
using UnityEngine.InputSystem;
using VComponent.InputSystem;

namespace VComponent.CameraSystem
{
    public class CameraController : MonoBehaviour
    {
        [Header("Parameters")] 
        [SerializeField] private CameraParameters _parameters;
        
        [Header("Components")]
        [SerializeField] private Transform _cameraTransform;

        [Header("Shared Values")] 
        [SerializeField] private SOSharedBool _isPlacementModeSharedValue;
        
        private Camera _mainCam;
        
        private Vector3 _newPosition;
        private Quaternion _newYRotation;
        private Quaternion _newXRotation;
        private Vector3 _newZoom;

        private Vector3 _dragMovementStartPosition;
        private Vector3 _dragMovementCurrentPosition;
        
        private Vector2 _dragRotateStartPosition;
        private Vector2 _dragRotateCurrentPosition;
        
        private bool _dragMovementInitialized;
        private bool _dragRotationInitialized;

        private void Awake()
        {
            _mainCam = Camera.main;

            transform.localPosition = new Vector3(_parameters.StartPosition.x, 0, _parameters.StartPosition.z);
            transform.rotation = Quaternion.Euler(0, _parameters.StartRotation.y, 0);
            
            _cameraTransform.localPosition = new Vector3(0, _parameters.StartPosition.y, 0);
            _cameraTransform.localRotation = Quaternion.Euler(_parameters.StartRotation.x, 0, 0);
            
            _newPosition = transform.position;
            _newYRotation = transform.rotation;
            _newXRotation = _cameraTransform.localRotation;
            _newZoom = _cameraTransform.localPosition;
        }

        private void Update()
        {
            HandleDragInput();
            HandleMovementInput();
        }
        
        private void HandleDragInput()
        {
            if (InputsManager.Instance.DragMovementCamera)
            {
                // First frame input
                if (!_dragMovementInitialized)
                {
                    Plane plane1 = new Plane(Vector3.up, Vector3.zero);
                    Ray ray1 = _mainCam.ScreenPointToRay(Input.mousePosition);

                    if (plane1.Raycast(ray1, out var entry1))
                    {
                        _dragMovementStartPosition = ray1.GetPoint(entry1);
                    }
                    
                    _dragMovementInitialized = true;
                }
                
                Plane plane = new Plane(Vector3.up, Vector3.zero);
                Ray ray = _mainCam.ScreenPointToRay(Input.mousePosition);
                if (plane.Raycast(ray, out var entry))
                {
                    _dragMovementCurrentPosition = ray.GetPoint(entry);
                    _newPosition = transform.position + _dragMovementStartPosition - _dragMovementCurrentPosition;
                }
            }
            else
            {
                _dragMovementInitialized = false;
            }
            
            if (InputsManager.Instance.DragRotationCamera)
            {
                // First frame input
                if (!_dragRotationInitialized)
                {
                    // Reset previous rotation since it may not be reached because of the lerp.
                    _newXRotation = _cameraTransform.localRotation;
                    _newYRotation = transform.rotation;
                    
                    _dragRotateStartPosition = Mouse.current.position.ReadValue();
                    _dragRotationInitialized = true;
                }

                _dragRotateCurrentPosition = Mouse.current.position.ReadValue();

                // PITCH
                if (!_parameters.LockPitch)
                {
                    // Computing the drag vertical input
                    var differenceY = (_dragRotateStartPosition.y - _dragRotateCurrentPosition.y) / Screen.height;
                    differenceY = Mathf.Clamp(differenceY, -1, 1);
                
                    if (differenceY != 0 && Mathf.Abs(differenceY) > _parameters.PitchDeadZone)
                    {
                        differenceY = _parameters.InversePitch ? -differenceY : differenceY;
                        _newXRotation *= Quaternion.Euler(Vector3.right * (differenceY * _parameters.RotationSensitivity));
                        _newXRotation = _newXRotation.ClampAxis(ExtensionMethods.Axis.X, _parameters.PitchRange.x, _parameters.PitchRange.y);
                    }
                }

                // YAW
                if (!_parameters.LockYaw)
                {
                    // Computing the drag horizontal input
                    var differenceX = (_dragRotateStartPosition.x - _dragRotateCurrentPosition.x) / Screen.width;
                    differenceX = Mathf.Clamp(differenceX, -1, 1);
                
                    if (differenceX != 0)
                    {
                        _newYRotation *= Quaternion.Euler(Vector3.up * (-Mathf.Sign(differenceX) * _parameters.RotationSensitivity));
                        _newYRotation = _newYRotation.ClampAxis(ExtensionMethods.Axis.Y, _parameters.RotationClamp.x, _parameters.RotationClamp.y);
                    }
                }
            }
            else
            {
                _dragRotationInitialized = false;
            }
        }

        private void HandleMovementInput()
        {
            // Movement
            var movementSpeed = InputsManager.Instance.QuickMoveCamera ? _parameters.FastMovementSpeed : _parameters.NormalMovementSpeed;

            // Calculate movement direction relative to camera rotation
            var movementDirection = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0) * new Vector3(InputsManager.Instance.MoveCamera.x, 0, InputsManager.Instance.MoveCamera.y);
            _newPosition += movementDirection.normalized * movementSpeed;

            // Clamp the position to the allowed range in _parameters
            _newPosition.z = Mathf.Clamp(_newPosition.z, _parameters.PositionRange.zMin, _parameters.PositionRange.zMax);
            _newPosition.x = Mathf.Clamp(_newPosition.x, _parameters.PositionRange.xMin, _parameters.PositionRange.xMax);
            
            transform.position = Vector3.Lerp(transform.position, _newPosition, _parameters.MovementResponsiveness * Time.deltaTime);

            // ROTATION KEYBOARD
            if (!_parameters.LockYaw && InputsManager.Instance.ClockwiseRotationCamera)
            {
                _newYRotation *= Quaternion.Euler(Vector3.up * _parameters.RotationSensitivity);
                _newYRotation = _newYRotation.ClampAxis(ExtensionMethods.Axis.Y, _parameters.RotationClamp.x, _parameters.RotationClamp.y);
                
                transform.rotation = Quaternion.Lerp(transform.rotation, _newYRotation, _parameters.RotationResponsiveness * Time.deltaTime);
            }
            if (!_parameters.LockYaw && InputsManager.Instance.AntiClockwiseRotationCamera)
            {
                _newYRotation *= Quaternion.Euler(Vector3.up * -_parameters.RotationSensitivity);
                _newYRotation = _newYRotation.ClampAxis(ExtensionMethods.Axis.Y, _parameters.RotationClamp.x, _parameters.RotationClamp.y);
                
                transform.rotation = Quaternion.Lerp(transform.rotation, _newYRotation, _parameters.RotationResponsiveness * Time.deltaTime);
            }

            // ROTATION MOUSE
            if (InputsManager.Instance.DragRotationCamera)
            {
                if (!_parameters.LockPitch)
                {
                    // X Rotation PITCH
                    _cameraTransform.localRotation = Quaternion.Lerp(_cameraTransform.localRotation, _newXRotation, _parameters.RotationResponsiveness * Time.deltaTime);
                }

                if (!_parameters.LockYaw)
                {
                    // Y Rotation YAW
                    transform.rotation = Quaternion.Lerp(transform.rotation, _newYRotation, _parameters.RotationResponsiveness * Time.deltaTime);
                }
            }

            // Zoom
            if (InputsManager.Instance.ScrollMouse != 0 && !_isPlacementModeSharedValue.IsTrue)
            {
                var zoomDelta = new Vector3(0, InputsManager.Instance.ScrollMouse * _parameters.ZoomSpeed, 0);

                _newZoom = _parameters.InvertZoom ? _cameraTransform.position + zoomDelta : _cameraTransform.position - zoomDelta;
                _newZoom.y = Mathf.Clamp(_newZoom.y, _parameters.ZoomClamp.x, _parameters.ZoomClamp.y);
                
                _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, _newZoom, _parameters.ZoomResponsiveness * Time.deltaTime);
            }
        }

        /// <summary>
        /// Set the camera position by an extern call
        /// </summary>
        public void SetCameraPosition(Vector3 position)
		{
            _newPosition = position;
		}
    }
}