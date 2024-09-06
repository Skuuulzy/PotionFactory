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
        [SerializeField] private MachineType _type;

        [SerializeField] private GameObject _gridView;
        [SerializeField] private Sprite _uiView;
        [SerializeField] private MachineBehavior _behavior;

        [Header("Structure")] 
        [SerializeField] private List<Node> _nodes;

        [Header("Parameters")] 
        [SerializeField] private int _maxItemCount;

        [Header("Shop")]
        [SerializeField] private float _shopSpawnProbability;
        [SerializeField] private int _shopPrice = 200;
        
        public string Name => _name;
        public MachineType Type => _type;

        public GameObject GridView => _gridView;
        public Sprite UIView => _uiView;

        public List<Node> Nodes => GetNodeInstance();
        public int MaxItemCount => _maxItemCount;
        public float ShopSpawnProbability => _shopSpawnProbability;
        public int ShopPrice => _shopPrice;

        
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