using System;
using System.Collections.Generic;
using Components.Grid;
using UnityEngine;

namespace Components.Machines
{
    public static class MachineExtensionsMethods
    {
        // ------------------------------------------------------------------------- NODE ROTATION ---------------------------------------------------
        public static List<Node> RotateNodes(this List<Node> nodes, int angle)
        {
            var rotationMapping = GetRotationMapping(angle);

            foreach (var node in nodes)
            {
                // Update the local position of the node.
                var newPosition = RotatePosition(node.LocalPosition, angle);
                node.UpdateLocalPosition(newPosition);

                // Update the direction of the ports.
                foreach (var port in node.Ports)
                {
                    var newSide = rotationMapping[port.Side];
                    port.UpdateSide(newSide);
                }
            }

            return nodes;
        }
        
        /// <summary>
        /// Rotate the local position of the node around is origin.
        /// </summary>
        /// <example>
        /// I have a matrices 2x2 with the coordinates are: [(0,0), (1,0), (0,-1), (1,-1)], now if a rotation of 90° is applied, the matrice is now: [(0,0), (0,-1), (-1,0),(-1,-1)]
        /// </example>
        private static Vector2Int RotatePosition(Vector2Int position, int angle)
        {
            switch (angle)
            {
                case 90:
                    return new Vector2Int(position.y, -position.x);
                case 180:
                    return new Vector2Int(-position.x, -position.y);
                case 270:
                    return new Vector2Int(-position.y, position.x);
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
                    throw new ArgumentException($"Invalid rotation angle: {angle}. Only 90, 180, and 270 degrees are allowed.");
            }

            return mapping;
        }
        
        // ------------------------------------------------------------------------- SIDE -------------------------------------------------------------
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

        public static int AngleFromSide(this Side side, bool inverted = false)
        {
            switch (side)
            {
                case Side.LEFT:
                case Side.RIGHT:
                    return 0;
                case Side.DOWN:
                    return inverted ? 90 : 270;
                case Side.UP:
                    return inverted ? 270 :90;
                    
                // This should be it but the preview arrow are not oriented correctly if used.   
                //case Side.RIGHT:
                    //return 180;
                
                default:
                    Debug.LogError($"Cannot determine an angle from the side:{side}");
                    return -1;
            }
        }

        public static Side SideFromAngle(this int angle)
        {
            switch (angle)
            {
                case 0:
                    return Side.RIGHT;
                case 90:
                    return Side.DOWN;
                case 180:
                    return Side.LEFT;
                case 270:
                    return Side.UP;
                default:
                    Debug.LogError($"Cannot determine a side from the angle:{angle}");
                    return Side.NONE;
            }
        }

        public static Vector2Int GetNeighbourPosition(this Side side, Vector2Int position)
        {
            switch (side)
            {
                case Side.DOWN:
                    return new Vector2Int(position.x, position.y - 1);
                case Side.UP:
                    return new Vector2Int(position.x, position.y + 1);
                case Side.RIGHT:
                    return new Vector2Int(position.x + 1, position.y);
                case Side.LEFT:
                    return new Vector2Int(position.x - 1, position.y);
                default:
                    Debug.LogError($"Cannot determine neighbour position at side: {side} from position: {position}");
                    return Vector2Int.zero;
            }
        }
        
        public static int NormalizeAngle(this int angle)
        {
            angle %= 360;
            if (angle < 0) angle += 360;
            return angle;
        }
        
        // ----------------------------------------------------------------------- GRID -------------------------------------------------------------
        
        public static void TryConnectPort(this Port port, Vector2Int neighbourPosition, Grid.Grid grid)
        {
            if (grid.TryGetCellByCoordinates(neighbourPosition.x, neighbourPosition.y, out Cell neighbourCell))
            {
                if (neighbourCell.ContainsNode)
                {
                    foreach (var potentialPort in neighbourCell.Node.Ports)
                    {
                        if (potentialPort.Side == port.Side.Opposite())
                        {
                            port.ConnectTo(potentialPort);
                        }
                    }
                }
                else
                {
                    port.Disconnect();
                }
            }
        }
        
        public static bool TryGetAllPotentialConnection(List<Node> machineNodes, Vector2Int gridPosition, Grid.Grid grid, out List<Port> potentialPorts)
        {
            potentialPorts = new List<Port>();
			
            foreach (var node in machineNodes)
            {
                foreach (var port in node.Ports)
                {
                    var potentialNeighbourPosition = new Vector2Int();
				
                    switch (port.Side)
                    {
                        case Side.DOWN:
                            potentialNeighbourPosition = new Vector2Int(gridPosition.x, gridPosition.y - 1);
                            break;
                        case Side.UP:
                            potentialNeighbourPosition = new Vector2Int(gridPosition.x, gridPosition.y + 1);
                            break;
                        case Side.RIGHT:
                            potentialNeighbourPosition = new Vector2Int(gridPosition.x + 1, gridPosition.y);
                            break;
                        case Side.LEFT:
                            potentialNeighbourPosition = new Vector2Int(gridPosition.x - 1, gridPosition.y);
                            break;
                        case Side.NONE:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (TryGetPotentialConnection(port, potentialNeighbourPosition, grid, out Port potentialPort))
                    {
                        potentialPorts.Add(potentialPort);
                    }
                }
            }

            return potentialPorts.Count > 0;
        }
        
        private static bool TryGetPotentialConnection(Port port, Vector2Int neighbourPosition, Grid.Grid grid, out Port potentialPort)
        {
            if (grid.TryGetCellByCoordinates(neighbourPosition.x, neighbourPosition.y, out Cell neighbourCell))
            {
                if (neighbourCell.ContainsNode)
                {
                    foreach (var neighbourPort in neighbourCell.Node.Ports)
                    {
                        if (neighbourPort.Side == port.Side.Opposite())
                        {
                            potentialPort = neighbourPort;
                            return true;
                        }
                    }
                }
            }

            potentialPort = null;
            return false;
        }
        
        // ----------------------------------------------------------------------- PLACEMENT -------------------------------------------------------------
        
        public static bool CanOverwrite(this Machine machineThatOverwrite, Cell cellToOverwrite)
        {
            // If the cell does not contain a machine it cannot be overwritten.
            if (!cellToOverwrite.ContainsNode)
            {
                return false;
            }

            var machineToOverwrite = cellToOverwrite.Node.Machine;
            
            // Check if the machine to overwrite can be retrieved and the machine to place can overwrite.
            if (!machineToOverwrite.Template.CanRetrieve || !machineThatOverwrite.Template.CanOverwriteOnPlacement)
            {
                return false;
            }

            return true;
        }
    }
}