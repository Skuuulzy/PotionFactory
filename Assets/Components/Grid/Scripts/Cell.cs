using System;
using Components.Grid.Obstacle;
using Components.Machines;
using UnityEngine;

namespace Components.Grid
{
    [Serializable]
    public class Cell
    {
        public int X { get; }
        public int Y { get; }
        public float Size { get; }
        public bool IsWater { get; private set; }
        public bool ContainsObject { get; private set; }
        public bool ContainsObstacle { get; private set; }

		[SerializeField] private ObstacleController _obstacle; 
		[SerializeField] private Node _node; 
       
        public ObstacleController Obstacle => _obstacle;  
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
            _obstacle = obstacle;
            ContainsObject = true;
            ContainsObstacle = true;
        }

        public void RemoveObstacleFromCell()
        {
            _obstacle = null;
            ContainsObject = false;
            ContainsObstacle = false;
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

        public void DefineCellAsWaterCell()
		{
            IsWater = true;
		}
    }
}