using System.Collections.Generic;
using Components.Machines.Behaviors;
using UnityEngine;

namespace Components.Machines
{
    [CreateAssetMenu(fileName = "New Machine Template", menuName = "Machines/Machine Template")]
    public class MachineTemplate : ScriptableObject
    {
        [Header("Definition")]
        [SerializeField] private MachineType _type;
        [SerializeField] private GameObject _3dView;
        [SerializeField] private MachineBehavior _behavior;
        
        [Header("Ports")]
        [SerializeField] private List<Side> _inPorts;
        [SerializeField] private  List<Side> _outPorts;
        
        [Header("Parameters")]
        [SerializeField] private int _maxItemCount;

        
        public MachineType Type => _type;
        public MachineBehavior Behavior => _behavior;

        public GameObject View => _3dView;

        public List<Side> InPorts => _inPorts;
        public List<Side> OutPorts => _outPorts;
        public int MaxItemCount => _maxItemCount;
    }
}