using System.Collections.Generic;
using UnityEngine;

public partial class MapService
{
    public const int NumTriesAbort = 500;

    public static IntVector3[] Directions = {
        new IntVector3(1, 0),
        new IntVector3(1, -1),
        new IntVector3(0, -1),
        new IntVector3(-1, -1),
        new IntVector3(-1, 0),
        new IntVector3(-1, 1),
        new IntVector3(0, 1),
        new IntVector3(1, 1)
    };

    public static List<IntVector3> GetPositionsWithinRadius(int minDistance, IntVector3 start, int radius) {
        // setup for checks
        List<TileNode> toBeVisited = new List<TileNode>();
        List<IntVector3> alreadyVisited = new List<IntVector3>();

        if (!LevelDataManager.Instance.IsWithinMap(start)) {
            CustomLogger.Warn(nameof(MapService), $"Starting position '{start}' is out of bounds!");
            return null;
        }

        List<IntVector3> traversableTargets = new List<IntVector3>();
        TileNode current = new TileNode() {
            X = start.x,
            Y = start.y,
            DistanceFromStart = 0
        };

        toBeVisited.Add(current);
        int count = 0;

        while (toBeVisited.Count != 0) {
            count++;
            current = toBeVisited[0];
            TryAddToList(minDistance, traversableTargets, current);
            toBeVisited.RemoveAt(0);
            alreadyVisited.Add(new IntVector3(current.X, current.Y));

            // check all directions
            for (int i = 0; i < Directions.Length; i++) {
                int dirX = Directions[i].x;
                int dirY = Directions[i].y;
                int neighborX = current.X + Directions[i].x;
                int neighborY = current.Y + Directions[i].y;
                IntVector3 neighbor = new IntVector3(neighborX, neighborY);
                int distanceFromStart = DistanceFromStart(start.x, start.y, neighborX, neighborY);
                if (distanceFromStart > radius) {
                    continue;
                } // stay within the radius
                if(!LevelDataManager.Instance.IsWithinMap(neighbor)) {
                    continue;
                } // don't check outside of map
                if (alreadyVisited.Exists(x => x == neighbor)) {
                    continue;
                } // don't re-attempt tiles we've already checked

                // ensure that this tile is traversable
                ITileInfo neighborTileInfo = LevelDataManager.Instance.GetTileAt(neighborX, neighborY);
                bool _canTraverse = neighborTileInfo != null
                    && neighborTileInfo.Occupants.Count == 0
                    && !neighborTileInfo.Data.IsSolid;

                if (!_canTraverse || ContainsNode(neighborX, neighborY, toBeVisited)) {
                    continue;
                }

                // add this tile as a place to check
                TileNode newNode = new TileNode() {
                    X = neighborX,
                    Y = neighborY,
                    DistanceFromStart = distanceFromStart
                };
                toBeVisited.Add(newNode);
            }

            // failsafe in case we end up in an infinite loop
            if (count > NumTriesAbort) {
                CustomLogger.Error(nameof(MapService), $"{nameof(GetTraversableTiles)} Aborting after {count} steps!");
                break;
            }
        }

        return traversableTargets;
    }

    public static List<IntVector3> GetTraversableTiles(
        int radius,
        IntVector3 start,
        ITileOccupant occupant,
        int traversableThreshold,
        int minDistance = 0) {
        List<TileNode> toBeVisited = new List<TileNode>();
        List<IntVector3> alreadyVisited = new List<IntVector3>();

        if(!LevelDataManager.Instance.IsWithinMap(start)) {
            CustomLogger.Warn(nameof(MapService), $"Starting position '{start}' is out of bounds!");
            return null;
        }

        List<IntVector3> traversableTargets = new List<IntVector3>();
        TileNode current = new TileNode() {
            X = start.x,
            Y = start.y,
            DistanceFromStart = 0
        };

        toBeVisited.Add(current);
        int count = 0;

        while(toBeVisited.Count != 0) {
            count++;
            current = toBeVisited[0];
            ITileInfo currentTile = LevelDataManager.Instance.GetTileAt(current.X, current.Y);
            if(current.DistanceFromStart >= minDistance && IsTileTraversable(currentTile, occupant, traversableThreshold)) {
                traversableTargets.Add(new IntVector3(current.X, current.Y));
            }
            toBeVisited.RemoveAt(0);
            alreadyVisited.Add(new IntVector3(current.X, current.Y));

            for(int i = 0; i < Directions.Length; i++) {
                int dirX = Directions[i].x;
                int dirY = Directions[i].y;
                int neighborX = current.X + Directions[i].x;
                int neighborY = current.Y + Directions[i].y;
                IntVector3 neighbor = new IntVector3(neighborX, neighborY);

                int distanceFromStart = DistanceFromStart(start.x, start.y, neighborX, neighborY);
                if (distanceFromStart > radius) {
                    continue;
                }

                if (ContainsIntVector3(neighborX, neighborY, alreadyVisited) || !LevelDataManager.Instance.IsWithinMap(neighbor)) {
                    continue;
                }

                ITileInfo neighborTileInfo = LevelDataManager.Instance.GetTileAt(neighborX, neighborY);
                bool _canTraverse = IsTileTraversable(neighborTileInfo, occupant, traversableThreshold);

                // if this is a corner piece
                int sumOf = Mathf.Abs(dirX) + Mathf.Abs(dirY);
                if (sumOf == 2 && _canTraverse) {
                    // check if adjacent sides are open
                    ITileInfo neighborTileX = LevelDataManager.Instance.GetTileAt(current.X + dirX, current.Y);
                    ITileInfo neighborTileY = LevelDataManager.Instance.GetTileAt(current.X, current.Y + dirY);
                    // check if both tiles are available
                    _canTraverse &= IsTileTraversable(neighborTileX, occupant, traversableThreshold);
                    _canTraverse &= IsTileTraversable(neighborTileY, occupant, traversableThreshold);
                }

                if (ContainsNode(neighborX, neighborY, toBeVisited)) {
                    continue;
                }

                TileNode newNode = new TileNode() {
                    X = neighborX,
                    Y = neighborY,
                    DistanceFromStart = distanceFromStart
                };
                toBeVisited.Add(newNode);
            }
            if(count > NumTriesAbort) {
                CustomLogger.Error(nameof(MapService), $"{nameof(GetTraversableTiles)} Aborting after {count} steps!");
                break;
            }
        }
        return traversableTargets;
    }
    
    // add this to the list if at the proper distance
    private static void TryAddToList(int minDistance, List<IntVector3> list, TileNode node) {
        if (node.DistanceFromStart >= minDistance) {
            list.Add(new IntVector3(node.X, node.Y));
        }
    }

    private static bool ContainsNode(int x, int y, List<TileNode> toBeVisited) {
        for (int i = 0; i < toBeVisited.Count; i++) {
            if (toBeVisited[i].X == x && toBeVisited[i].Y == y) {
                return true;
            }
        }
        return false;
    }

    private static bool ContainsIntVector3(int x, int y, List<IntVector3> list) {
        for(int i = 0; i < list.Count; i++) {
            if(list[i].x == x && list[i].y == y) {
                return true;
            }
        }
        return false;
    }
}
