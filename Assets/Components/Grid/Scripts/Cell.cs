using System;
using Components.Grid.Obstacle;
using Components.Grid.Tile;
using Components.Machines;
using UnityEngine;

namespace Components.Grid
{
    public class Cell
    {
        public int X { get; }
        public int Y { get; }
        public float Size { get; }
        public bool ContainsObject { get; private set; }
        public bool ContainsObstacle { get; private set; }
        public bool ContainsTile { get; private set; }

		[SerializeField] private ObstacleController _obstacleController; 
		[SerializeField] private TileController _tileController; 
		[SerializeField] private Node _node; 
       
        public ObstacleController ObstacleController => _obstacleController;  
        public TileController TileController => _tileController;  
        public Node Node => _node;

        public Cell(int x, int y, float size, bool containsObject)
        {
            X = x;
            Y = y;
            Size = size;
            ContainsObject = containsObject;
        }

        public void AddObstacleToCell(ObstacleController obstacle)
        {
            _obstacleController = obstacle;
            ContainsObject = true;
            ContainsObstacle = true;
        }

        public void RemoveObstacleFromCell()
        {
            _obstacleController = null;
            ContainsObject = false;
            ContainsObstacle = false;
        }

		public void AddTileToCell(TileController tile)
		{
			_tileController = tile;
			ContainsTile = true;
		}

		public void RemoveTileFromCell()
		{
			_tileController = null;
			ContainsTile = false;
		}


		public void AddNodeToCell(Node node)
        {
            ContainsObject = true;
            _node = node;
        }

        public void RemoveNodeFromCell()
        {
            ContainsObject = false;
            _node = null;
        }


    }


    [Serializable]
    public class SerializedCell
	{
        [SerializeField] public int X;
        [SerializeField] public int Y;
        [SerializeField] public float Size;
        [SerializeField] public bool ContainsObject;
        [SerializeField] public bool ContainsObstacle;
        [SerializeField] public bool ContainsTile;
        [SerializeField] public TileType TileType;
        [SerializeField] public ObstacleType ObstacleType;


        public SerializedCell(Cell cell)
		{
            X = cell.X;
            Y = cell.Y;
            Size = cell.Size;
            ContainsObject = cell.ContainsObject;
            ContainsObstacle = cell.ContainsObstacle;
            ContainsTile = cell.ContainsTile;

            
            TileType = cell.TileController == null ? TileType.NONE : cell.TileController.TileType ;
            ObstacleType = cell.ObstacleController == null ? ObstacleType.NONE : cell.ObstacleController.ObstacleType ;
		}
    }
}