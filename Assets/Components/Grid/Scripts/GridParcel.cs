using System;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Grid
{
    [Serializable]
    public struct GridParcel
    {
        public Vector2Int OriginPosition;
        public int Lenght;
        public int Width;


        public List<Vector2Int> Coordinates()
        {
            List<Vector2Int> _coordinates = new List<Vector2Int>();

            for (int x = 0; x < Lenght; x++)
            {
                for (int y = 0; y < Width; y++)
                {
                    _coordinates.Add(new Vector2Int(OriginPosition.x + x, OriginPosition.y + y));
                }
            }

            return _coordinates;
        }
    }
}