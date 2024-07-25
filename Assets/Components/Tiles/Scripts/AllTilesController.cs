using System.Collections.Generic;
using UnityEngine;

namespace Components.Grid.Tile
{
    public class AllTilesController : ScriptableObject
    {
        [SerializeField] private List<TileController> _groundTilesList;
        private TileController _groundTile;
        [SerializeField] private TileController _waterTile;
        [SerializeField] private float _waterTileGenerationProbability;

        public TileController GenerateTile(Cell cell, Grid grid, Transform groundHolder, float cellSize)
        {
            if (!(Random.value <= _waterTileGenerationProbability))
            {
                TileController tile = Instantiate(_groundTile, groundHolder);
                tile.transform.position = grid.GetWorldPosition(cell.X, cell.Y) + new Vector3(cellSize / 2, 0, cellSize / 2);
                tile.transform.localScale = new Vector3(cellSize, cellSize, cellSize);
                tile.name = $"Cell ({cell})";
                tile.SetTileType(_groundTile.TileType);
                cell.AddTileToCell(tile);
                return tile;
            }
            else
            {
                TileController tile = Instantiate(_waterTile, groundHolder);
                tile.transform.position = grid.GetWorldPosition(cell.X, cell.Y) + new Vector3(cellSize / 2, 0, cellSize / 2);
                tile.transform.localScale = new Vector3(cellSize, cellSize, cellSize);
                tile.name = $"Cell ({cell})";
                tile.SetTileType(TileType.WATER);
				cell.AddTileToCell(tile);
                return tile;
            }

        }

        public TileController GenerateTileFromPrefab(Cell cell, Grid grid, Transform groundHolder, float cellSize, TileController tileController)
		{
            TileController tile = Instantiate(tileController, groundHolder);
            tile.transform.position = grid.GetWorldPosition(cell.X, cell.Y) + new Vector3(cellSize / 2, 0, cellSize / 2);
            //tile.transform.localScale = new Vector3(cellSize, cellSize, cellSize);
            tile.name = $"Cell ({cell})";
            tile.SetTileType(tileController.TileType);
			return tile;
        }

        public void SelectATileType()
        {
            _groundTile = _groundTilesList[Random.Range(0, _groundTilesList.Count)];
        }
    }
}
