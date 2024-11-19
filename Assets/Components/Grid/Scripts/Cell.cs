using System;
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
        public int X { get; }
        public int Y { get; }
        public float Size { get; }
        public bool ContainsObject { get; private set; }
        public bool ContainsNode { get; private set; }
        public bool ContainsObstacle { get; private set; }
        public bool ContainsTile { get; private set; }
        public bool ContainsIngredient { get; private set; }

		[SerializeField] private ObstacleController _obstacleController; 
		[SerializeField] private TileController _tileController; 
		[SerializeField] private Node _node; 
		[SerializeField] private IngredientTemplate _ingredient;

		[SerializeField] private List<RelicEffect> _relicEffects; 
        [SerializeField] private List<DecorationController> _decorations;
       
        public ObstacleController ObstacleController => _obstacleController;  
        public TileController TileController => _tileController;  
        public Node Node => _node;
        public IngredientTemplate Ingredient => _ingredient;
        public List<RelicEffect> RelicEffects => _relicEffects;

        public Cell(int x, int y, float size, bool containsObject)
        {
            X = x;
            Y = y;
            Size = size;
            ContainsObject = containsObject;
            _relicEffects = new List<RelicEffect>();
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

        public void AddIngredientToCell(IngredientTemplate ingredientTemplate)
        {
	        ContainsObject = true;
	        ContainsIngredient = true;
	        _ingredient = ingredientTemplate;
        }

        public void RemoveIngredientFromCell()
        {
	        ContainsObject = false;
	        ContainsIngredient = false;
	        _ingredient = null;
        }

        public void AddRelicEffectToCell(RelicEffect effect)
        {
            _relicEffects.Add(effect);
            if(_node != null)
			{
                effect.ApplyEffect(_node.Machine.Behavior);
			}
        }

        public void AddDecorationToCell(DecorationController decoration)
        {
            _decorations.Add(decoration);
        }

        public Vector3 GetCenterPosition(Vector3 originPosition)
        {
	        return new Vector3(X + Size / 2, 0, Y + Size / 2) * Size + originPosition;
        }

        public Vector2Int Position => new(X, Y);
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