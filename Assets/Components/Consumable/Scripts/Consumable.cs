using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Components.Consumable
{

	[Serializable]
	public class Consumable 
	{

		// ----------------------------------------------------------------------- PRIVATE FIELDS -------------------------------------------------------------------------
		[SerializeField, ReadOnly] private ConsumableController _controller;

		private readonly ConsumableTemplate _template;

		// ----------------------------------------------------------------------- PUBLIC FIELDS -------------------------------------------------------------------------
		public ConsumableTemplate Template => _template;
		public ConsumableController Controller => _controller;



		// --------------------------------------------------------------------- INITIALISATION -------------------------------------------------------------------------
		public Consumable(ConsumableTemplate template, ConsumableController controller)
		{
			_template = template;
			_controller = controller;
		}

	}
}
