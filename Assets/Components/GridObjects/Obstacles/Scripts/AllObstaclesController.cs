
using Components.Grid.Tile;
using System.Collections.Generic;
using UnityEngine;


namespace Components.Grid.Obstacle
{
	public class AllObstaclesController : ScriptableObject
	{
		[SerializeField] private List<ObstacleController> _obstacleList;
		[SerializeField] private float _obstacleGenerationProbability;
		[SerializeField] private float _randomBonusProbability;

		public ObstacleController GenerateObstacle(Grid grid, Cell chosenCell, Transform obstacleHolder, float cellSize)
		{
			_randomBonusProbability = 0.0f;

			if (!(Random.value <= _obstacleGenerationProbability + _randomBonusProbability))
			{
				return null;
			}

			var obstacle = Instantiate(_obstacleList[Random.Range(0, _obstacleList.Count)], obstacleHolder);
			obstacle.transform.position = grid.GetWorldPosition(chosenCell.X, chosenCell.Y) + new Vector3(cellSize / 2, 0, cellSize / 2);
			obstacle.transform.localScale = new Vector3(cellSize, cellSize, cellSize);

			chosenCell.AddObstacleToCell(obstacle);
			
			Debug.Log($"Instantiating obstacle on ({chosenCell.X}, {chosenCell.Y})");
			
			return obstacle;
		}

		public ObstacleController GenerateObstacleFromPrefab(Grid grid, Cell chosenCell, Transform obstacleHolder, float cellSize, ObstacleController obstacleController, bool freePlacement)
		{
			ObstacleController obstacle = Instantiate(obstacleController, obstacleHolder);
			if (freePlacement)
			{
				obstacle.transform.position = obstacleController.transform.position;
			}
			else
			{

				obstacle.transform.position = grid.GetWorldPosition(chosenCell.X, chosenCell.Y) + new Vector3(cellSize / 2, obstacleController.transform.position.y, cellSize / 2);
			}
			obstacle.transform.localScale = obstacleController.transform.localScale;
			obstacle.transform.rotation = obstacleController.transform.rotation;


			obstacle.SetObstacleType(obstacleController.ObstacleType);
			chosenCell.AddObstacleToCell(obstacle);
			return obstacle;
		}

		/// <summary>
		/// Generates an obstacle from a given type, position, rotation, and scale, and assigns it to the cell.
		/// </summary>
		public ObstacleController GenerateObstacleFromType(Cell chosenCell, Grid grid, Transform obstacleHolder, float cellSize, ObstacleType obstacleType, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
		{
			foreach (ObstacleController obstacleController in _obstacleList)
			{
				if (obstacleController.ObstacleType == obstacleType)
				{
					// Instantiate the obstacle
					ObstacleController obstacle = Instantiate(obstacleController, obstacleHolder);

					// Set position, rotation, and scale
					obstacle.transform.position = localPosition;
					obstacle.transform.localRotation = localRotation;
					obstacle.transform.localScale = localScale;

					// Set obstacle type and assign to cell
					obstacle.SetObstacleType(obstacleType);
					chosenCell.AddObstacleToCell(obstacle);

					return obstacle;
				}
			}

			Debug.LogError("Can't find obstacle associated with ObstacleType: " + obstacleType);
			return null;
		}

	}


}

