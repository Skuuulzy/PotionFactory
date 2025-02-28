using System;
using System.Collections.Generic;
using Components.Grid.Decorations;
using Components.Grid.Obstacle;
using Components.Grid.Tile;
using UnityEngine;

namespace Components.Grid
{
    [Serializable]
    public class SerializedCell
    {
        public int X;
        public int Y;
        public float Size;
        public bool ContainsObject;
        public bool ContainsObstacle;
        public bool ContainsTile;
        public TileType TileType;
        public ObstacleType ObstacleType;
        public float[] ObstacleRotation;
        public float[] ObstacleScale;
        public DecorationType[] DecorationTypes;
        public List<float[]> DecorationPositions;
        public List<float[]> DecorationRotations;
        public List<float[]> DecorationScales;

        public SerializedCell() { }

        public SerializedCell(Cell cell)
        {
            X = cell.Coordinates.x;
            Y = cell.Coordinates.y;
            Size = cell.Size;
            ContainsObject = cell.ContainsGridObject;
            ContainsObstacle = cell.ContainsObstacle;
            ContainsTile = cell.ContainsTile;
            TileType = cell.TileController == null ? TileType.NONE : cell.TileController.TileType;
            ObstacleType = cell.ObstacleController == null ? ObstacleType.NONE : cell.ObstacleController.ObstacleType;

            if (cell.ObstacleController != null)
            {
                // Serialize obstacle rotation and scale
                Quaternion rotation = cell.ObstacleController.transform.localRotation;
                ObstacleRotation = new float[] { rotation.x, rotation.y, rotation.z, rotation.w };

                Vector3 scale = cell.ObstacleController.transform.localScale;
                ObstacleScale = new float[] { scale.x, scale.y, scale.z };
            }

            if (cell.DecorationControllers != null)
            {
                DecorationTypes = new DecorationType[cell.DecorationControllers.Count];
                DecorationPositions = new List<float[]>();
                DecorationRotations = new List<float[]>();
                DecorationScales = new List<float[]>();

                for (int i = 0; i < cell.DecorationControllers.Count; i++)
                {
                    DecorationTypes[i] = cell.DecorationControllers[i].DecorationType;

                    // Serialize decoration position
                    Vector3 position = cell.DecorationControllers[i].transform.localPosition;
                    DecorationPositions.Add(new float[] { position.x, position.y, position.z });

                    // Serialize decoration rotation
                    Quaternion rotation = cell.DecorationControllers[i].transform.localRotation;
                    DecorationRotations.Add(new float[] { rotation.x, rotation.y, rotation.z, rotation.w });

                    // Serialize decoration scale
                    Vector3 scale = cell.DecorationControllers[i].transform.localScale;
                    DecorationScales.Add(new float[] { scale.x, scale.y, scale.z });
                }
            }
        }
    }
}