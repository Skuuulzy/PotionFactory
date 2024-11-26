using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Grid.Decorations
{
    public class DecorationController : GridObjectController
    {
		[SerializeField] private DecorationType _decorationType;

		public DecorationType DecorationType => _decorationType;

		// ------------------------------------------------------------------------- INIT -------------------------------------------------------------------------

		public void SetDecorationType(DecorationType obstacleType)
		{
			_decorationType = obstacleType;
		}
	}

	public enum DecorationType
	{
		NONE,
		BLUE_FLOWER
	}
}
