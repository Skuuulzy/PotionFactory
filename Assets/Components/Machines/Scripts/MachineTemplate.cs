using System.Collections.Generic;
using System.Linq;
using Components.Machines.Behaviors;
using Components.Machines.UIView;
using UnityEngine;

namespace Components.Machines
{
    [CreateAssetMenu(fileName = "New Machine Template", menuName = "Component/Machines/Machine Template")]
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
        
        [Header("Process")]
        [SerializeField] private bool _canTakeInfiniteIngredients;
        [SerializeField] private int _inSlotIngredientCount = 1;
        [SerializeField] private int _outSlotIngredientCount = 1;
        [SerializeField] private int _ingredientsPerSlotCount = 1;
        [SerializeField] private int _processTime;
        
        [Header("Shop")]
        [SerializeField] private float _shopSpawnProbability;
        [SerializeField] private int _shopPrice = 200;
        [SerializeField] private bool _cannotBeSell;
        [SerializeField] private int _sellPrice = 100;

        [Header("Contextual View")] 
        [SerializeField] private List<UIContextualComponent> _contextualComponents;
        [SerializeField] private string _uiGameplayDescription;
        [SerializeField] private string _uiLoreDescription;
        
        public string Name => _name;
        public MachineType Type => _type;

        public GameObject GridView => _gridView;
        public Sprite UIView => _uiView;

        public List<Node> Nodes => GetNodeInstance();

        public bool CanTakeInfiniteIngredients => _canTakeInfiniteIngredients;
        public int InSlotIngredientCount => _inSlotIngredientCount;
        public int OutSlotIngredientCount => _outSlotIngredientCount;
        public int IngredientsPerSlotCount => _ingredientsPerSlotCount;

        public int ProcessTime => _processTime;

        public float ShopSpawnProbability => _shopSpawnProbability;
        public int ShopPrice => _shopPrice;
        public int SellPrice => _sellPrice;
        public bool CannotBeSell => _cannotBeSell;

        public List<UIContextualComponent> ContextualComponents => _contextualComponents;
        public string UIGameplayDescription => _uiGameplayDescription;
        public string UILoreDescription => _uiLoreDescription;
        
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
        
        /// Return the size of the machine based on his nodes.
        public (int width, int length) Size()
        {
            int minX = _nodes.Min(node => node.LocalPosition.x);
            int maxX = _nodes.Max(node => node.LocalPosition.x);
            int minY = _nodes.Min(node => node.LocalPosition.y);
            int maxY = _nodes.Max(node => node.LocalPosition.y);
        
            int width = maxX - minX + 1;
            int length = maxY - minY + 1;
        
            return (width, length);
        }
        
        /// Return the coordinates of the center of the machine.
        public (float X, float Z) Center()
        {
            int minX = _nodes.Min(node => node.LocalPosition.x);
            int maxX = _nodes.Max(node => node.LocalPosition.x);
            int minY = _nodes.Min(node => node.LocalPosition.y);
            int maxY = _nodes.Max(node => node.LocalPosition.y);

            float centerX = (minX + maxX) / 2f;
            float centerZ = (minY + maxY) / 2f;

            return (centerX, centerZ);
        }
    }
}