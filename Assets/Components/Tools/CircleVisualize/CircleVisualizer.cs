using CodeMonkey.Utils;
using Components.Grid.Generator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleVisualizer : MonoBehaviour
{
	// The radius of the circle
	[SerializeField] private GridGenerator _gridGenerator;
	[SerializeField] private int _circleSegments = 360;
	// The color of the circle in the scene view
	[SerializeField] private Color _circleColor = Color.green;

	// The camera reference to convert screen space to world space
	[SerializeField] private Camera _camera;


	private void OnDrawGizmos()
	{
		// Only draw the circle if the game is in Play mode
		if (!Application.isPlaying || !_gridGenerator.CleanMode)
		{
			return;
		}

		if (_camera == null)
		{
			_camera = Camera.main;
		}

		// Convert the mouse position to world position
		Vector3 worldMousePosition = Vector3.zero;
		if (UtilsClass.ScreenToWorldPositionIgnoringUI(Input.mousePosition, _camera, out worldMousePosition))
		{
			// Set the Gizmos color
			Gizmos.color = _circleColor;

			// Draw a circle (a wireframe sphere with a very low height)
			DrawCircle(worldMousePosition, _gridGenerator.CleanRadius);
		}
	}

	private void DrawCircle(Vector3 center, float radius)
	{
		float angleStep = 360f / _circleSegments;

		// Loop through the segments to create the circle
		Vector3 previousPoint = Vector3.zero;
		for (int i = 0; i < _circleSegments; i++)
		{
			// Calculate the angle in radians
			float angle = Mathf.Deg2Rad * i * angleStep;

			// Calculate the new point on the circle
			Vector3 point = center + new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);

			// If it's not the first point, draw a line from the previous point to the current point
			if (i > 0)
			{
				Gizmos.DrawLine(previousPoint, point);
			}

			previousPoint = point;
		}

		// Draw a line from the last point to the first to complete the circle
		Gizmos.DrawLine(previousPoint, center + new Vector3(Mathf.Cos(0) * radius, 0f, Mathf.Sin(0) * radius));
	}

}
