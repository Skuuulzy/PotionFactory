using UnityEngine;

namespace Components.Machines
{
    [System.Serializable]
    public class Port
    {
        [SerializeField] private Side _side;
        [SerializeField] private Type _type;

        public Side Side => _side;

        public enum Type
        {
            IN,
            OUT
        }
    }
}