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
		Book_01,
		Book_02,
		Bottle_Clay_01,
		Bottle_Clay_02,
		Bottle_Clay_03,
		Bottle_Clay_04,
		BottlePotion_01,
		BottlePotion_02,
		BottlePotion_03,
		Candle,
		Carpet_Large,
		Carpet_Small,
		Carpet_Table,
		Etandar_01,
		Etandar_02,
		Etandar_03,
		Etandar_04,
		Etandar_Shop,
		Parchemin,
		Planks_01,
		Planks_02,
		Planks_03,
		Mushroom_Brown,
		Mushroom_Brown_Group,
		Mushroom_Red,
		Mushroom_Red_Group,
		Mushroom_Yellow,
		Mushroom_Yellow_Group,

	}
}
