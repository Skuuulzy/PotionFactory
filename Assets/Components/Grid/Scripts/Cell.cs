using System;
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
        public bool ContainsObject { get; private set; }

		[SerializeField] private GameObject _obstacle; 
		[SerializeField] private Node _node; 
       
        public GameObject Obstacle => _obstacle;  
        public Node Node => _node;

        public Cell(int x, int y, float size, bool containsObject)
        {
            X = x;
            Y = y;
            Size = size;
            ContainsObject = containsObject;
        }

        public void AddObstacleToCell(GameObject obstacle)
        {
            _obstacle = obstacle;
            ContainsObject = true;
        }

        public void RemoveObstacleFromCell()
        {
            _obstacle = null;
            ContainsObject = false;
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
}