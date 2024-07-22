using Components.Grid;
using Components.Machines;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : ScriptableObject
{
    [SerializeField] private List<GameObject> _obstacleList;
    [SerializeField] private float _obstacleGenerationProbability;
    [SerializeField] private float _randomBonusProbability;

	public void GenerateObstacle(Components.Grid.Grid grid, Cell chosenCell, Transform obstacleHolder, float cellSize)
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
}
