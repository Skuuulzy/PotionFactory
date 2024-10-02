using Components.Grid;
using Components.Machines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Relics.Behavior
{
	public abstract class RelicBehavior : ScriptableObject
	{
		[SerializeField] private string _description;

		public string Description => _description;

		public RelicBehavior Clone()
		{
			return Instantiate(this);
		}
	}
}

