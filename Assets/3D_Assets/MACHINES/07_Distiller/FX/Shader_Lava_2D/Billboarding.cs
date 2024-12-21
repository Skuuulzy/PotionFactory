using UnityEngine;

public class Bilboarding : MonoBehaviour
{
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
    }

    private void LateUpdate()
    {
        if (!_initialized)
        {
            return;
        }
        
        _cameraDirection = _cameraTransform.forward;
        _cameraDirection.y = 0;

        transform.rotation = Quaternion.LookRotation(_cameraDirection);
    }
}
