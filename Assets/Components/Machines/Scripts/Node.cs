using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Components.Machines
{
    [Serializable]
    public class Node
    {
        [SerializeField] private Vector2Int _localPosition;
        [SerializeField, ReadOnly] private Vector2Int _gridPosition;
        [SerializeField] private List<Port> _ports;

        [NonSerialized] private Machine _machine;
        
        public Vector2Int LocalPosition => _localPosition;
        public Vector2Int GridPosition => _gridPosition;

        public List<Port> Ports => _ports;
        public Machine Machine => _machine;

        public Node(Node copy)
        {
            _localPosition = copy._localPosition;
            _gridPosition = copy._gridPosition;

            List<Port> ports = new List<Port>();
            foreach (var port in copy._ports)
            {
                ports.Add(new Port(port));
            }

            _ports = ports;
        }

        public void SetMachine(Machine machine)
        {
            _machine = machine;
            foreach (var port in _ports)
            {
                port.SetNode(this);
            }
        }
        
        public void UpdateLocalPosition(Vector2Int newPosition)
        {
            _localPosition = newPosition;
        }

        public Vector2Int GetGridPosition(Vector2Int originCoordinates)
        {
            _gridPosition = originCoordinates + _localPosition;
            return _gridPosition;
        }
    }
}