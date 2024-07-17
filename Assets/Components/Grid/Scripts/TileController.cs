using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : ScriptableObject
{
    [SerializeField] private List<GameObject> _groundTilesList;
    private GameObject _groundTile;
    [SerializeField] private GameObject _waterTile;
    [SerializeField] private float _waterTileGenerationProbability;

    public bool GenerateTile(int x, int z, Components.Grid.Grid grid, Transform groundHolder, float cellSize)
    {
        if (!(Random.value <= _waterTileGenerationProbability))
        {
            var tile = Instantiate(_groundTile, grid.GetWorldPosition(x, z), Quaternion.identity, groundHolder);
            tile.transform.localScale = new Vector3(cellSize, cellSize, cellSize);
            tile.name = $"Cell ({x}, {z})";
            return false;
        }
        else
        {
            var tile = Instantiate(_waterTile, grid.GetWorldPosition(x, z), Quaternion.identity, groundHolder);
            tile.transform.localScale = new Vector3(cellSize, cellSize, cellSize);
            tile.name = $"Cell ({x}, {z})";
            grid.TryGetCellByCoordinates(x, z, out var cell);
            cell.DefineCellAsWaterCell();

            return true;
        }

    }

    public void SelectATileType()
	{
        _groundTile = _groundTilesList[Random.Range(0, _groundTilesList.Count)];

    }
}
