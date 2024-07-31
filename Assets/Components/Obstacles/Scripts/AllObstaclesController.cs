
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
				return null;
			}


			var obstacle = Instantiate(_obstacleList[UnityEngine.Random.Range(0, _obstacleList.Count)], obstacleHolder);
			obstacle.transform.position = grid.GetWorldPosition(chosenCell.X, chosenCell.Y) + new Vector3(cellSize / 2, 0, cellSize / 2);
			obstacle.transform.localScale = new Vector3(cellSize, cellSize, cellSize);

			chosenCell.AddObstacleToCell(obstacle);
			return obstacle;
		}

		public ObstacleController GenerateObstacleFromPrefab(Grid grid, Cell chosenCell, Transform obstacleHolder, float cellSize, ObstacleController obstacleController)
		{
			ObstacleController obstacle = Instantiate(obstacleController, obstacleHolder);
			obstacle.transform.position = grid.GetWorldPosition(chosenCell.X, chosenCell.Y) + new Vector3(cellSize / 2, 0, cellSize / 2);
			obstacle.transform.localScale = new Vector3(cellSize, cellSize, cellSize);


			obstacle.SetObstacleType(obstacleController.ObstacleType);
			chosenCell.AddObstacleToCell(obstacle);
			return obstacle;
		}

		public ObstacleController GenerateObstacleFromType( Cell chosenCell, Grid grid, Transform obstacleHolder, float cellSize, ObstacleType obstacleType)
		{
			foreach (ObstacleController obstacleController in _obstacleList)
			{
				if (obstacleController.ObstacleType == obstacleType)
				{
					ObstacleController obstacle = Instantiate(obstacleController, obstacleHolder);
					obstacle.transform.position = grid.GetWorldPosition(chosenCell.X, chosenCell.Y) + new Vector3(cellSize / 2, 0, cellSize / 2);
					obstacle.transform.localScale = new Vector3(cellSize, cellSize, cellSize);


					obstacle.SetObstacleType(obstacleType);
					chosenCell.AddObstacleToCell(obstacle);
					return obstacleController;
				}
			}

			Debug.LogError("Can't find obstacle associated ObstacleType : " + obstacleType);
			return null;
		}
	}


}

