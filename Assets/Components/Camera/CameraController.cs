using UnityEngine;

namespace Components.Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float _movementSpeed;

        private Vector3 _cameraPosition;

        private void Update()
        {
            HandleInputs();
            HandleMovement();
        }

        private void HandleInputs()
        {
            float cameraSpeed = 100f;

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                _cameraPosition += new Vector3(-1, 0, 0) * (cameraSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                _cameraPosition += new Vector3(+1, 0, 0) * (cameraSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                _cameraPosition += new Vector3(0, 0, +1) * (cameraSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                _cameraPosition += new Vector3(0, 0, -1) * (cameraSpeed * Time.deltaTime);
            }
        }

        private void HandleMovement()
        {
            _cameraPosition.y = transform.position.y;

            Vector3 cameraMoveDir = (_cameraPosition - transform.position).normalized;
            float distance = Vector3.Distance(_cameraPosition, transform.position);

            if (distance > 0)
            {
                Vector3 newCameraPosition = transform.position + cameraMoveDir * (distance * _movementSpeed * Time.deltaTime);

                float distanceAfterMoving = Vector3.Distance(newCameraPosition, _cameraPosition);

                if (distanceAfterMoving > distance)
                {
                    // Overshot the target
                    newCameraPosition = _cameraPosition;
                }

                transform.position = newCameraPosition;
            }
        }
    }
}