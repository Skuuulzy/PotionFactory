using System.Collections.Generic;
using UnityEngine;

namespace Components.Grid.Tile
{
    public class AllTilesController : ScriptableObject
    {
        [SerializeField] private List<TileController> _groundTilesList;
        [SerializeField] private TileController _waterTile;
        [SerializeField] private float _waterTileGenerationProbability;

        private TileController _groundTile;
        
        public TileController GenerateTile(Cell cell, Grid grid, Transform groundHolder, float cellSize)
        {
            if (!(Random.value <= _waterTileGenerationProbability))
            {
                TileController tile = Instantiate(_groundTile, groundHolder);
                tile.transform.position = grid.GetWorldPosition(cell.X, cell.Y) + new Vector3(cellSize / 2, 0, cellSize / 2);
                tile.transform.localScale = new Vector3(cellSize, cellSize, cellSize);
                tile.SetTileType(_groundTile.TileType);
                cell.AddTileToCell(tile);
                tile.name = $"Tile ({tile.TileType})";
                return tile;
            }
            else
            {
                TileController tile = Instantiate(_waterTile, groundHolder);
                tile.transform.position = grid.GetWorldPosition(cell.X, cell.Y) + new Vector3(cellSize / 2, 0, cellSize / 2);
                tile.transform.localScale = new Vector3(cellSize, cellSize, cellSize);
                tile.SetTileType(TileType.WATER);
				cell.AddTileToCell(tile);
                tile.name = $"Tile ({tile.TileType})";
                return tile;
            }

        }

        public TileController GenerateTileFromPrefab(Cell cell, Grid grid, Transform groundHolder, float cellSize, TileController tileController)
		{
            TileController tile = Instantiate(tileController, groundHolder);
            tile.transform.position = grid.GetWorldPosition(cell.X, cell.Y) + new Vector3(cellSize / 2, 0, cellSize / 2);
            tile.transform.localScale = new Vector3(cellSize, cellSize, cellSize);
            tile.name = $"Tile ({tile})";
            tile.SetTileType(tileController.TileType);
			return tile;
        }

        public TileController GenerateTileFromType(Cell cell, Grid grid, Transform groundHolder, float cellSize, TileType tileType)
		{
            if(tileType == TileType.WATER)
			{
                TileController tile = Instantiate(_waterTile, groundHolder);
                tile.transform.position = grid.GetWorldPosition(cell.X, cell.Y) + new Vector3(cellSize / 2, 0, cellSize / 2);
                tile.transform.localScale = new Vector3(cellSize, cellSize, cellSize);
                tile.name = $"Tile ({tile})";
                tile.SetTileType(TileType.WATER);
                cell.AddTileToCell(tile);
                return tile;
			}

            foreach(TileController tileController in _groundTilesList)
			{
                if(tileController.TileType == tileType)
				{
                    TileController tile = Instantiate(tileController, groundHolder);
                    tile.transform.position = grid.GetWorldPosition(cell.X, cell.Y) + new Vector3(cellSize / 2, 0, cellSize / 2);
                    tile.transform.localScale = new Vector3(cellSize, cellSize, cellSize);
                    tile.name = $"Tile ({tile})";
                    tile.SetTileType(tileType);
                    cell.AddTileToCell(tile);
                    return tile;
				}
			}

            Debug.LogError("Can't find tile associated TileType : " + tileType);
            return null;
		}


        public void SelectATileType()
        {
            _groundTile = _groundTilesList[Random.Range(0, _groundTilesList.Count)];
        }
    }
}
