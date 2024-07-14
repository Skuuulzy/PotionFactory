using Sirenix.OdinInspector;
using UnityEngine;

namespace Components.Machines
{
    [System.Serializable]
    public class Port
    {
        [SerializeField] private Side _side;
        [SerializeField] private Type _type;
        [ShowInInspector] private Port _connectedPort;

        public Side Side => _side;
        
        public Port(Port copy)
        {
            _side = copy._side;
            _type = copy._type;
            _connectedPort = copy._connectedPort;
        }

        public void UpdateSide(Side newSide)
        {
            _side = newSide;
        }

        public void SetConnectedPort(Port connectedPort)
        {
            _connectedPort = connectedPort;
        }

        public enum Type
        {
            IN,
            OUT
        }
    }
}