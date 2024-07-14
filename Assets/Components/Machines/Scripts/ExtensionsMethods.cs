using System;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Machines
{
    public static class ExtensionsMethods
    {
        public static List<Node> RotateNodes(this List<Node> ports, int angle, Vector2Int dimensions)
        {
            var rotationMapping = GetRotationMapping(angle);
            var rotatedPorts = new List<Node>();

            foreach (var port in ports)
            {
                var newPosition = RotatePosition(port.Position, angle, dimensions);
                var newSide = rotationMapping[port.Side];
                rotatedPorts.Add(new Node(newSide, newPosition));
            }

            return rotatedPorts;
        }
        
        private static Vector2Int RotatePosition(Vector2Int position, int angle, Vector2Int dimensions)
        {
            switch (angle)
            {
                case 90:
                    return new Vector2Int(-position.y, position.x);
                case 180:
                    return new Vector2Int(-position.x, -position.y);
                case 270:
                    return new Vector2Int(position.y, -position.x);
                default:
                    throw new ArgumentException("Invalid rotation angle. Only 90, 180, and 270 degrees are allowed.");
            }
        }

        private static Dictionary<Side, Side> GetRotationMapping(int angle)
        {
            var mapping = new Dictionary<Side, Side>();

            switch (angle)
            {
                case 90:
                    mapping[Side.UP] = Side.RIGHT;
                    mapping[Side.RIGHT] = Side.DOWN;
                    mapping[Side.DOWN] = Side.LEFT;
                    mapping[Side.LEFT] = Side.UP;
                    mapping[Side.NONE] = Side.NONE;
                    break;
                case 180:
                    mapping[Side.UP] = Side.DOWN;
                    mapping[Side.RIGHT] = Side.LEFT;
                    mapping[Side.DOWN] = Side.UP;
                    mapping[Side.LEFT] = Side.RIGHT;
                    mapping[Side.NONE] = Side.NONE;
                    break;
                case 270:
                    mapping[Side.UP] = Side.LEFT;
                    mapping[Side.RIGHT] = Side.UP;
                    mapping[Side.DOWN] = Side.RIGHT;
                    mapping[Side.LEFT] = Side.DOWN;
                    mapping[Side.NONE] = Side.NONE;
                    break;
                default:
                    throw new ArgumentException("Invalid rotation angle. Only 90, 180, and 270 degrees are allowed.");
            }

            return mapping;
        }
        
        public static Side Opposite(this Side side)
        {
            switch (side)
            {
                case Side.DOWN:
                    return Side.UP;
                case Side.UP:
                    return Side.DOWN;
                case Side.RIGHT:
                    return Side.LEFT;
                case Side.LEFT:
                    return Side.RIGHT;
                case Side.NONE:
                    return Side.NONE;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}