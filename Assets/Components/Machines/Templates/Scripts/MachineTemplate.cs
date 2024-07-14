using System.Collections.Generic;
using Components.Machines.Behaviors;
using UnityEngine;

namespace Components.Machines
{
    [CreateAssetMenu(fileName = "New Machine Template", menuName = "Machines/Machine Template")]
    public class MachineTemplate : ScriptableObject
    {
        [Header("Definition")] [SerializeField]
        private string _name;

        [SerializeField] private GameObject _gridView;
        [SerializeField] private Sprite _uiView;
        [SerializeField] private MachineBehavior _behavior;

        [Header("Structure")] 
        [SerializeField] private List<Node> _baseInPorts;
        [SerializeField] private List<Node> _baseOutPorts;
        [SerializeField] private List<Node> _nodes;

        [Header("Parameters")] [SerializeField]
        private int _maxItemCount;

        public string Name => _name;

        public GameObject GridView => _gridView;
        public Sprite UIView => _uiView;

        public List<Node> Nodes => GetNodeInstance();

        public List<Node> BaseInPorts => _baseInPorts;
        public List<Node> BaseOutPorts => _baseOutPorts;
        public int MaxItemCount => _maxItemCount;


        private List<Node> GetNodeInstance()
        {
            List<Node> result = new List<Node>();
            
            foreach (var node in _nodes)
            {
                Node newNode = new Node(node);
                result.Add(newNode);
            }

            return result;
        }
        
        public MachineBehavior GetBehaviorClone()
        {
            return _behavior.Clone();
        }
    }
}