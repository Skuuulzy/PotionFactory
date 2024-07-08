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
        
        [Header("Ports")]
        [SerializeField] private List<Side> _baseInPorts;
        [SerializeField] private  List<Side> _baseOutPorts;
        
        [Header("Parameters")]
        [SerializeField] private int _maxItemCount;
        
        public string Name => _name;
        
        public GameObject GridView => _gridView;
        public Sprite UIView => _uiView;

        public List<Side> BaseInPorts => _baseInPorts;
        public List<Side> BaseOutPorts => _baseOutPorts;
        public int MaxItemCount => _maxItemCount;

        public MachineBehavior GetBehaviorClone()
        {
            return _behavior.Clone();
        }
    }
}