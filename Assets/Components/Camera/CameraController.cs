using UnityEngine;

namespace Components.Camera
{
    public class CameraController : MonoBehaviour
	{
		[SerializeField] private float _cameraSpeed = 10f;
		[SerializeField] private float _responsiveness = 5f;
		[SerializeField] private float _rotationSpeed = 100f;

		private Vector3 _cameraPosition;
		private float _speedMultiplicateur = 1f;
		private void Start()
		{
			_cameraPosition = transform.position;
		}

		private void Update()
		{
			HandleInputs();
			HandleMovement();
			HandleCameraRotation();
		}

		/// <summary>
		/// Handles the camera movement inputs.
		/// </summary>
		private void HandleInputs()
		{
			Vector3 forward = transform.forward;
			Vector3 right = transform.right;

			if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
			{
				_cameraPosition -= right * (_cameraSpeed * _speedMultiplicateur * Time.deltaTime);
			}

			if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
			{
				_cameraPosition += right * (_cameraSpeed * _speedMultiplicateur * Time.deltaTime);
			}

			if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
			{
				_cameraPosition += forward * (_cameraSpeed * _speedMultiplicateur * Time.deltaTime);
			}

			if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
			{
				_cameraPosition -= forward * (_cameraSpeed * _speedMultiplicateur * Time.deltaTime);
			}

			if (Input.GetKeyDown(KeyCode.LeftShift))
			{
				_speedMultiplicateur = 2;
			}
			if (Input.GetKeyUp(KeyCode.LeftShift))
			{
				_speedMultiplicateur = 1;
			}
		}

		/// <summary>
		/// Handles the camera movement towards the target position.
		/// </summary>
		private void HandleMovement()
		{
			_cameraPosition.y = transform.position.y;

			Vector3 cameraMoveDir = (_cameraPosition - transform.position).normalized;
			float distance = Vector3.Distance(_cameraPosition, transform.position);

			if (distance > 0)
			{
				Vector3 newCameraPosition = transform.position + cameraMoveDir * (distance * _responsiveness * Time.deltaTime);

				float distanceAfterMoving = Vector3.Distance(newCameraPosition, _cameraPosition);

				if (distanceAfterMoving > distance)
				{
					// Overshot the target
					newCameraPosition = _cameraPosition;
				}

				transform.position = newCameraPosition;
			}
		}

		/// <summary>
		/// Handles the camera rotation based on keyboard input.
		/// </summary>
		private void HandleCameraRotation()
		{
			if (Input.GetKey(KeyCode.Q))
			{
				RotateCamera(-_rotationSpeed);
			}
			else if (Input.GetKey(KeyCode.E))
			{
				RotateCamera(_rotationSpeed);
			}
		}

		/// <summary>
		/// Rotates the camera by the specified speed.
		/// </summary>
		/// <param name="speed">The speed at which to rotate the camera.</param>
		private void RotateCamera(float speed)
		{
			transform.Rotate(Vector3.up, speed * Time.deltaTime, Space.World);
		}
	}
}