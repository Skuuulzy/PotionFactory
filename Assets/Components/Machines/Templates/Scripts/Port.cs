using System;
using UnityEngine;

namespace Components.Machines
{
    [Serializable]
    public struct Port
    {
        [SerializeField] private Side _side;
        [SerializeField] private Vector2Int _position;

        public Side Side => _side;
        public Vector2Int Position => _position;
        
        public Port(Side side, Vector2Int position)
        {
            _side = side;
            _position = position;
        }
    }
}