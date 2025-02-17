using System.Collections.Generic;
using Components.Grid.Decorations;
using Components.Grid.Obstacle;
using Components.Grid.Tile;
using Components.Ingredients;
using Components.Machines;
using UnityEngine;

namespace Components.Grid
{
    public class Cell
    {
        public Vector2Int Coordinates { get; }
        public float Size { get; }
        public bool ContainsObject { get; private set; }
        public bool ContainsNode { get; private set; }
        public bool ContainsObstacle { get; private set; }
        public bool ContainsTile { get; private set; }
        public bool Unlocked { get; private set; }

        
        // TODO: Remove all sub controllers to use only the GridObjectController and cast
        private GridObjectController _gridObjectController;
		private ObstacleController _obstacleController; 
		private TileController _tileController; 
		private Node _node; 
		private IngredientTemplate _ingredient;
		private List<DecorationController> _decorationControllers;
       
        public ObstacleController ObstacleController => _obstacleController;  
        public TileController TileController => _tileController;
        public List<DecorationController> DecorationControllers => _decorationControllers;
        public Node Node => _node;

        public Cell(int x, int y, float size, bool containsObject)
        {
            Coordinates = new Vector2Int(x, y);
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
		
		public void AddNodeToCell(Node node)
        {
            ContainsNode = true;
            ContainsObject = true;
            _node = node;
        }

        public void RemoveNodeFromCell()
        {
	        ContainsNode = false;
            ContainsObject = false;
            _node = null;
        }
        
        public void Unlock()
        {
	        Unlocked = true;
        }

        public void AddDecorationToCell(DecorationController decoration)
        {
            if(_decorationControllers == null)
            {
                _decorationControllers = new List<DecorationController>();
            }

            _decorationControllers.Add(decoration);
        }

        public bool DetectDecorationOnCell(DecorationController decoration)
		{
            if(_decorationControllers != null)
			{
                foreach(DecorationController controller in _decorationControllers)
				{
                    if(controller.DecorationType == decoration.DecorationType)
					{
                        return true;
					}
				}
			}
            return false;
		}

        public void RemoveDecorationFromCell(DecorationController decoration)
        {
            _decorationControllers.Remove(decoration);
        }
        
        public Vector3 GetCenterPosition(Vector3 originPosition)
        {
	        return new Vector3(Coordinates.x + Size / 2, 0, Coordinates.y + Size / 2) * Size + originPosition;
        }

        public bool IsConstructable()
        {
	        if (!Unlocked || ContainsObject)
	        {
		        return false;
	        }

	        if (_tileController) 
	        {
		        if (_tileController.TileType == TileType.WATER)
		        {
			        return false;
		        }
	        }

	        return true;
        }
    }
}