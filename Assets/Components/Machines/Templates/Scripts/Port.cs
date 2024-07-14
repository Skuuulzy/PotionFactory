using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Components.Machines
{
    [System.Serializable]
    public class Port
    {
        [SerializeField] private Side _side;
        [SerializeField] private Way _way;
        [ShowInInspector, ReadOnly] private Port _connectedPort;

        [NonSerialized] private Node _node;
        
        public Side Side => _side;
        public Way Way => _way;
        public Node Node => _node;
        public Port ConnectedPort => _connectedPort;

        public Port(Port copy)
        {
            _side = copy._side;
            _way = copy._way;
            _connectedPort = copy._connectedPort;
        }

        public void UpdateSide(Side newSide)
        {
            _side = newSide;
        }
        
        public void SetNode(Node node)
        {
            _node = node;
        }

        public void SetConnectedPort(Port connectedPort)
        {
            // The two port have the same connection type
            if (_way == connectedPort.Way || _connectedPort == connectedPort)
            {
                return;
            }
            
            _connectedPort = connectedPort;
            // Tell to the other port too.
            connectedPort.SetConnectedPort(this);
        }


    }
}