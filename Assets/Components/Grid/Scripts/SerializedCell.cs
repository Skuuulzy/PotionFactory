using System;
using System.Collections.Generic;
using Components.Grid.Decorations;
using Components.Grid.Obstacle;
using Components.Grid.Tile;
using Newtonsoft.Json;
using UnityEngine;

namespace Components.Grid
{
    [Serializable]
    public class SerializedCell
    {
        [JsonProperty("X")]
        public int X { get; set; }

        [JsonProperty("Y")]
        public int Y { get; set; }

        [JsonProperty("Size")]
        public float Size { get; set; }

        [JsonProperty("ContainsObject")]
        public bool ContainsObject { get; set; }

        [JsonProperty("ContainsObstacle")]
        public bool ContainsObstacle { get; set; }

        [JsonProperty("ContainsTile")]
        public bool ContainsTile { get; set; }

        [JsonProperty("TileType")]
        public TileType TileType { get; set; }

        [JsonProperty("ObstacleType")]
        public ObstacleType ObstacleType { get; set; }

        [JsonProperty("ObstacleRotation")]
        public float[] ObstacleRotation { get; set; }

        [JsonProperty("ObstacleScale")]
        public float[] ObstacleScale { get; set; }

        [JsonProperty("DecorationTypes")]
        public DecorationType[] DecorationTypes { get; set; }

        [JsonProperty("DecorationPositions")]
        public List<float[]> DecorationPositions { get; set; }

        [JsonProperty("DecorationRotations")]
        public List<float[]> DecorationRotations { get; set; }

        [JsonProperty("DecorationScales")]
        public List<float[]> DecorationScales { get; set; }

        public SerializedCell() { }

        public SerializedCell(Cell cell)
        {
            X = cell.X;
            Y = cell.Y;
            Size = cell.Size;
            ContainsObject = cell.ContainsObject;
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