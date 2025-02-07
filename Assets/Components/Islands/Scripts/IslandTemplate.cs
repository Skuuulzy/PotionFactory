using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Island
{
    [CreateAssetMenu(fileName = "New Island Template", menuName = "Component/Island/Island Template")]
    public class IslandTemplate : ScriptableObject
    {
        [SerializeField] private string _name;
        [SerializeField] private UIIslandController _uIIslandController;
        public string Name => _name;
        public UIIslandController Controller => _uIIslandController;
    }
}
