using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that helps build paths for NPCs and enemies
/// </summary>
public partial class Mapservice
{
    private static List<TileNode> _toBeVisited = new List<TileNode>();
    private static List<IntVector3> _alreadyVisited = new List<IntVector3>();

    public const int NumTriesAbort = 100;

    private static IntVector3[] _directions = {
        new IntVector3(1, 0),
        new IntVector3(1, -1),
        new IntVector3(0, -1),
        new IntVector3(-1, -1),
        new IntVector3(-1, 0),
        new IntVector3(-1, 1),
        new IntVector3(0, 1),
        new IntVector3(1, 1)
    };

    public static PathStatus GetPathToDestination(IntVector3 startPosition, IntVector3 targetDestination, out List<IntVector3> path) {
        path = new List<IntVector3>();

        int startX = startPosition.x;
        int startY = startPosition.y;
        int targetX = targetDestination.x;
        int targetY = targetDestination.y;
        _toBeVisited.Clear();
        _alreadyVisited.Clear();

        TileNode current = new TileNode() {
            X = startX,
            Y = startY,
            DistanceFromStart = 0,
            TotalCost = 0
        };
        _toBeVisited.Add(current);
        int count = 0;
        while(_toBeVisited.Count != 0) {
            // get current node
            count++;
            current = GetLowestCostNode();
            // remove from to be visited
            _toBeVisited.Remove(current);

            // if we've hit our destination
            if (current.X == targetX && current.Y == targetY) {
                while(current.Parent != null) {
                    path.Insert(0, new IntVector3(current.X, current.Y));
                    current = current.Parent;
                }
                return PathStatus.Complete;
            }
            for(int i = 0; i < _directions.Length; i++) {
                int dirX =_directions[i].x;
                int dirY = _directions[i].y;
                int neighborX = current.X + dirX;
                int neighborY = current.Y + dirY;
                if (_alreadyVisited.Contains(new IntVector3(neighborX, neighborY))) {
                    continue;
                }
                ITileInfo info = LevelDataManager.Instance.GetTileAt(neighborX, neighborY);
                bool _canTraverse = info != null && !info.Data.IsSolid;

                // if this is a corner piece
                int sumOf = Mathf.Abs(dirX) + Mathf.Abs(dirY);
                if (sumOf == 2 && _canTraverse) {
                    // check if adjacent sides are open
                    ITileInfo neighborTileX = LevelDataManager.Instance.GetTileAt(current.X + dirX, current.Y);
                    ITileInfo neighborTileY = LevelDataManager.Instance.GetTileAt(current.X, current.Y + dirY);
                    // check if both tiles are available
                    _canTraverse &= !neighborTileX.Data.IsSolid;
                    _canTraverse &= !neighborTileY.Data.IsSolid;
                }

                int distanceFromStart = DistanceFromStart(startX, startY, neighborX, neighborY);

                if (!_canTraverse) {
                    continue;
                }

                if (TryGetNode(neighborX, neighborY, out int index)) {
                    TileNode toBeVisitedNode = _toBeVisited[index];
                    if (distanceFromStart < toBeVisitedNode.DistanceFromStart) {
                        toBeVisitedNode.DistanceFromStart = distanceFromStart;
                        continue;
                    }
                }

                // create a new node and add to open list
                TileNode newNode = new TileNode() {
                    X = neighborX,
                    Y = neighborY,
                    TotalCost = GetNodeTotalCost(startX, startY, neighborX, neighborY, targetX, targetY),
                    DistanceFromStart = distanceFromStart,
                    Parent = current
                };
                _toBeVisited.Add(newNode);
            }
            _alreadyVisited.Add(new IntVector3(current.X, current.Y));

            if (count > NumTriesAbort) {
                CustomLogger.Error(nameof(MapService), $"{nameof(GetPathToDestination)} Aborting after {count} steps!");
                break;
            }
        }
        return PathStatus.Invalid;
    }

    private static bool TryGetNode(int x, int y, out int index) {
        for(int i = 0; i < _toBeVisited.Count; i++) {
            if(_toBeVisited[i].X == x && _toBeVisited[i].Y == y) {
                index = i;
                return true;
            }
        }
        index = -1;
        return false;
    }

    private static TileNode GetLowestCostNode() {
        if(_toBeVisited.Count == 0) {
            return null;
        }
        TileNode lowestCostNode = _toBeVisited[0];
        for(int i = 1; i < _toBeVisited.Count; i++) {
            if(_toBeVisited[i].TotalCost < lowestCostNode.TotalCost) {
                lowestCostNode = _toBeVisited[i];
            }
        }
        return lowestCostNode;
    }

    public static int GetNodeTotalCost(int startX, int startY, int currentX, int currentY, int destinationX, int destinationY) {
        int totalCost = DistanceFromStart(startX, startY, currentX, currentY) + HeuristicValue(currentX, currentY, destinationX, destinationY);
        return totalCost;
    }

    public static int DistanceFromStart(int startX, int startY, int posX, int posY) {
        return Mathf.RoundToInt(Mathf.Abs(posX - startX)) + Mathf.RoundToInt(Mathf.Abs(posY - startY));
    }

    public static int HeuristicValue(int currentX, int currentY, int destinationX, int destinationY) {
        int value = 0;
        value += Mathf.RoundToInt(Mathf.Pow(destinationX - currentX, 2));
        value += Mathf.RoundToInt(Mathf.Pow(destinationY - currentY, 2));
        return value;
    }
}

public class TileNode {
    public int X;
    public int Y;
    public TileNode Parent;
    public int TotalCost;
    public int DistanceFromStart;
}
