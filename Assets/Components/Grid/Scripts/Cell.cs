using System;
using System.Collections.Generic;
using Components.Grid.Decorations;
using Components.Grid.Obstacle;
using Components.Grid.Tile;
using Components.Ingredients;
using Components.Machines;
using Newtonsoft.Json;
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
        [SerializeField] private List<DecorationController> _decorationControllers;
       
        public ObstacleController ObstacleController => _obstacleController;  
        public TileController TileController => _tileController;  
        public Node Node => _node;
        public IngredientTemplate Ingredient => _ingredient;
        public List<RelicEffect> RelicEffects => _relicEffects;
        public List<DecorationController> DecorationControllers => _decorationControllers;

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

        public DecorationController GetDecorationController(Vector3 worldMousePosition, float offset)
        {
            foreach (var decoration in _decorationControllers)
            {
                if (decoration == null) continue;

                if (Vector3.Distance(decoration.transform.position, worldMousePosition) <= offset)
                {
                    return decoration;
                }
            }

            return null;
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
        [JsonProperty("X")]
        public int X { get; set; }

        [JsonProperty("Y")]
        public int Y { get; set; }

        [JsonProperty("Size")]
        public float Size { get; set; }

        [JsonProperty("ContainsObject")]
        public bool ContainsObject { get; set; }

        [JsonProperty("ContainsObstacle")]
        public bool ContainsObstacle { get; set; }

        [JsonProperty("ContainsTile")]
        public bool ContainsTile { get; set; }

        [JsonProperty("TileType")]
        public TileType TileType { get; set; }

        [JsonProperty("ObstacleType")]
        public ObstacleType ObstacleType { get; set; }

        [JsonProperty("DecorationTypes")]
        public DecorationType[] DecorationTypes { get; set; }

        [JsonProperty("DecorationPositions")]
        public List<float[]> DecorationPositions { get; set; }

        public SerializedCell() { }

        public SerializedCell(Cell cell)
        {
            X = cell.X;
            Y = cell.Y;
            Size = cell.Size;
            ContainsObject = cell.ContainsObject;
            ContainsObstacle = cell.ContainsObstacle;
            ContainsTile = cell.ContainsTile;
            TileType = cell.TileController == null ? TileType.NONE : cell.TileController.TileType;
            ObstacleType = cell.ObstacleController == null ? ObstacleType.NONE : cell.ObstacleController.ObstacleType;

            if (cell.DecorationControllers != null)
            {
                DecorationTypes = new DecorationType[cell.DecorationControllers.Count];
                DecorationPositions = new List<float[]>();

                for (int i = 0; i < cell.DecorationControllers.Count; i++)
                {
                    DecorationTypes[i] = cell.DecorationControllers[i].DecorationType;

                    Vector3 position = cell.DecorationControllers[i].transform.localPosition;
                    DecorationPositions.Add(new float[] { position.x, position.y, position.z });
                }
            }
        }
    }
}