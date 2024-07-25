using System.Collections;
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

        public TileController GenerateTile(int x, int z, Grid grid, Transform groundHolder, float cellSize)
        {
            if (!(Random.value <= _waterTileGenerationProbability))
            {
                TileController tile = Instantiate(_groundTile, grid.GetWorldPosition(x, z), Quaternion.identity, groundHolder);
                tile.transform.localScale = new Vector3(cellSize, cellSize, cellSize);
                tile.name = $"Cell ({x}, {z})";
                tile.SetTileType(_groundTile.TileType);
                tile.SetCoordinate(x, z);
                return tile;
            }
            else
            {
                TileController tile = Instantiate(_waterTile, grid.GetWorldPosition(x, z), Quaternion.identity, groundHolder);
                tile.transform.localScale = new Vector3(cellSize, cellSize, cellSize);
                tile.name = $"Cell ({x}, {z})";
                tile.SetCoordinate(x, z);
                tile.SetTileType(TileType.WATER);
                grid.TryGetCellByCoordinates(x, z, out var cell);
                cell.DefineCellAsWaterCell();

                return tile;
            }

        }

        public TileController GenerateTileByPrefab(int x, int z, Grid grid, Transform groundHolder, float cellSize, TileController tileController)
		{
            TileController tile = Instantiate(tileController, grid.GetWorldPosition(x, z), Quaternion.identity, groundHolder);
            //tile.transform.localScale = new Vector3(cellSize, cellSize, cellSize);
            tile.name = $"Cell ({x}, {z})";
            tile.SetTileType(tileController.TileType);
            tile.SetCoordinate(x, z);
            return tile;
        }

        public void SelectATileType()
        {
            _groundTile = _groundTilesList[Random.Range(0, _groundTilesList.Count)];
        }
    }
}
