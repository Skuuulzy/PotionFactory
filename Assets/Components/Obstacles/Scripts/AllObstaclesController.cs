
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

		public void GenerateObstacle(Grid grid, Cell chosenCell, Transform obstacleHolder, float cellSize)
		{
			_randomBonusProbability = 0.0f;

			//Dictionary<Side, Cell> neighboursCells = grid.GetNeighboursByPosition(chosenCell);
			//foreach (Cell cell in neighboursCells.Values)
			//{
			//	if (cell.Obstacle != null)
			//	{
			//		_randomBonusProbability += 0.4f;
			//	}
			//}

			if (!(Random.value <= _obstacleGenerationProbability + _randomBonusProbability))
			{
				return;
			}


			var obstacle = Instantiate(_obstacleList[UnityEngine.Random.Range(0, _obstacleList.Count)], obstacleHolder);
			obstacle.transform.position = grid.GetWorldPosition(chosenCell.X, chosenCell.Y) + new Vector3(cellSize / 2, 0, cellSize / 2);
			obstacle.transform.localScale = new Vector3(cellSize, cellSize, cellSize);

			chosenCell.AddObstacleToCell(obstacle);
		}

		public ObstacleController GenerateObstacleFromPrefab(Grid grid, Cell chosenCell, Transform obstacleHolder, float cellSize, ObstacleController obstacleController)
		{
			var obstacle = Instantiate(_obstacleList[Random.Range(0, _obstacleList.Count)], obstacleHolder);
			obstacle.transform.position = grid.GetWorldPosition(chosenCell.X, chosenCell.Y) + new Vector3(cellSize / 2, 0, cellSize / 2);
			obstacle.transform.localScale = new Vector3(cellSize, cellSize, cellSize);
			chosenCell.AddObstacleToCell(obstacle);

			obstacle.SetObstacleType(obstacleController.ObstacleType);
			obstacle.SetCell(chosenCell);
			return obstacle;
		}
	}


}

