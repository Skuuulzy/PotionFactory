using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Components.Machines
{
    [Serializable]
    public class Node
    {
        private Side _side;
        [SerializeField] private Vector2Int _localPosition;
        [SerializeField, ReadOnly] private Vector2Int _debugWorldPosition;
        [SerializeField] private List<Port> _ports;
        
        public Side Side => _side;
        public Vector2Int LocalPosition => _localPosition;

        public List<Port> Ports => _ports;

        public Node(Node copy)
        {
            _side = copy._side;
            _localPosition = copy._localPosition;
            _debugWorldPosition = copy._debugWorldPosition;

            List<Port> ports = new List<Port>();
            foreach (var port in copy._ports)
            {
                ports.Add(new Port(port));
            }

            _ports = ports;
        }

        public void UpdateLocalPosition(Vector2Int newPosition)
        {
            _localPosition = newPosition;
        }

        public Vector2Int GridPosition(Vector2Int originCoordinates)
        {
            _debugWorldPosition = originCoordinates + _localPosition;
            return originCoordinates + _localPosition;
        }
    }
}