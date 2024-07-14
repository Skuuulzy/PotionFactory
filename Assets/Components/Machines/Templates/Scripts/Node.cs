using System;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Machines
{
    [Serializable]
    public class Node
    {
        [SerializeField] private Side _side;
        [SerializeField] private Vector2Int _position;
        [SerializeField] private Node _connectedNode;
        [SerializeField] private List<Port> _ports;
        
        public Side Side => _side;
        public Vector2Int Position => _position;
        public Node ConnectedNode => _connectedNode;

        public List<Port> Ports => _ports;

        public Node(Side side, Vector2Int position)
        {
            _side = side;
            _position = position;
        }

        public void UpdateLocalPosition(Vector2Int newPosition)
        {
            _position = newPosition;
        }
        
        public void SetConnectedPort(Node connectedNode)
        {
            _connectedNode = connectedNode;
        }

        public Vector2Int GridPosition(Vector2Int originCoordinates)
        {
            return originCoordinates + _position;
        }
    }
}