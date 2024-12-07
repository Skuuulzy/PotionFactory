using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Grid
{
	public class GridObjectTemplate : ScriptableObject
	{
		[Header("Definition")]
		[SerializeField] protected string _name;

		[SerializeField] protected GameObject _gridView;
		[SerializeField] protected Sprite _uiView;

		public string Name => _name;
		public GameObject GridView => _gridView;
		public Sprite UIView => _uiView;
	}
}