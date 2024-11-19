using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Grid.Decorations
{
    [CreateAssetMenu(fileName = "New Decoration Template", menuName = "Grid/Decoration Template")]
    public class DecorationTemplate : ScriptableObject
    {
		[Header("Definition")]
		[SerializeField] private string _name;
		[SerializeField] private DecorationType _decorationType;

		[SerializeField] private GameObject _gridView;
		[SerializeField] private Sprite _uiView;

		public string Name => _name;
		public DecorationType DecorationType => _decorationType;
		public GameObject GridView => _gridView;
		public Sprite UIView => _uiView;
	}
}
