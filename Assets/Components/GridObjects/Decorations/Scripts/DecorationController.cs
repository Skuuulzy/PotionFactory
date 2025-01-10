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
		Bush_Fl_Green,
		Bush_Fl_Tile_Green,
		Bush_Fl_Purple,
		Bush_Fl_Tile_Purple,
		Bush_Fl_Yellow,
		Bush_Fl_Tile_Yellow,
		Clover,
		Clover_Large_Patch,
		Clover_Small_Patch,
		Clover_Tile_Border,
		Clover_Tile_Corner_IN,
		Clover_Tile_Corner_OUT,
		Clover_Tile_Fill,
		Fern_Plant_01,
		Fern_Plant_02,
		Flower_Blue,
		Flower_Blue_Large_Patch,
		Flower_Blue_Small_Patch,
		Flower_Blue_Tile_Boder,
		Flower_Blue_Tile_Corner_IN,
		Flower_Blue_Tile_Corner_OUT,
		Flower_Blue_Tile_Fill,
		Flower_Pink,
		Flower_Pink_Large_Patch,
		Flower_Pink_Small_Patch,
		Flower_Pink_Tile_Boder,
		Flower_Pink_Tile_Corner_IN,
		Flower_Pink_Tile_Corner_OUT,
		Flower_Pink_Tile_Fill,
		Flower_Purple,
		Flower_Purple_Large_Patch,
		Flower_Purple_Small_Patch,
		Flower_Purple_Tile_Boder,
		Flower_Purple_Tile_Corner_IN,
		Flower_Purple_Tile_Corner_OUT,
		Flower_Purple_Tile_Fill,
		Flower_Red,
		Flower_Red_Large_Patch,
		Flower_Red_Small_Patch,
		Flower_Red_Tile_Boder,
		Flower_Red_Tile_Corner_IN,
		Flower_Red_Tile_Corner_OUT,
		Flower_Red_Tile_Fill,
		Flower_White,
		Flower_White_Large_Patch,
		Flower_White_Small_Patch,
		Flower_White_Tile_Boder,
		Flower_White_Tile_Corner_IN,
		Flower_White_Tile_Corner_OUT,
		Flower_White_Tile_Fill,
		Flower_Yellow,
		Flower_Yellow_Large_Patch,
		Flower_Yellow_Small_Patch,
		Flower_Yellow_Tile_Boder,
		Flower_Yellow_Tile_Corner_IN,
		Flower_Yellow_Tile_Corner_OUT,
		Flower_Yellow_Tile_Fill,
		High_Flower_Blue,
		High_Flower_Orange,
		High_Flower_Pink,
		High_Flower_White,
		Nenuphar,
		Nenuphar_Flower,
		Nenuphar_Giant,
		Reeds,
		Reeds_Group,
		Reeds_Bush,


	}
}
