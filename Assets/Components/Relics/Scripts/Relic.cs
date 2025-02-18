using Components.Tick;
using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Components.Relics
{
	
    [Serializable]
	public class Relic : ITickable
	{

		// ----------------------------------------------------------------------- PRIVATE FIELDS -------------------------------------------------------------------------
		[SerializeField, ReadOnly] private RelicController _controller;

		private readonly RelicTemplate _template;

		// ----------------------------------------------------------------------- PUBLIC FIELDS -------------------------------------------------------------------------
		public RelicTemplate Template => _template;
		public RelicController Controller => _controller;

		// ------------------------------------------------------------------------- ACTIONS -------------------------------------------------------------------------
		public Action OnTick;

		// --------------------------------------------------------------------- INITIALISATION -------------------------------------------------------------------------
		public Relic(RelicTemplate template, RelicController controller)
		{
			_template = template;
			_controller = controller;
		}

		public void Tick()
		{
			OnTick?.Invoke();
		}

	}
}

