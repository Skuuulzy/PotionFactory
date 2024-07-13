using System.Collections.Generic;
using Components.Machines.Behaviors;
using UnityEngine;

namespace Components.Machines
{
    [CreateAssetMenu(fileName = "New Machine Template", menuName = "Machines/Machine Template")]
    public class MachineTemplate : ScriptableObject
    {
        [Header("Definition")]
        [SerializeField] private string _name;
        [SerializeField] private GameObject _gridView;
        [SerializeField] private Sprite _uiView;
        [SerializeField] private MachineBehavior _behavior;
        
        [Header("Structure")]
        [SerializeField] private Vector2Int _dimension;
        [SerializeField] private List<Port> _baseInPorts;
        [SerializeField] private  List<Port> _baseOutPorts;
        
        [Header("Parameters")]
        [SerializeField] private int _maxItemCount;
        
        public string Name => _name;
        
        public GameObject GridView => _gridView;
        public Sprite UIView => _uiView;

        public List<Port> BaseInPorts => _baseInPorts;
        public List<Port> BaseOutPorts => _baseOutPorts;
        public int MaxItemCount => _maxItemCount;
        public Vector2Int Dimension => _dimension;

        public MachineBehavior GetBehaviorClone()
        {
            return _behavior.Clone();
        }
    }
}