using System.Collections.Generic;
using UnityEngine;

public partial class MapService
{
    private static List<TileNode> _toBeVisited = new List<TileNode>();
    private static List<IntVector3> _alreadyVisited = new List<IntVector3>();

    public const int NumTriesAbort = 100;

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
        _toBeVisited.Clear();
        _alreadyVisited.Clear();

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

        _toBeVisited.Add(current);
        int count = 0;

        while (_toBeVisited.Count != 0) {
            count++;
            current = _toBeVisited[0];
            TryAddToList(minDistance, traversableTargets, current);
            _toBeVisited.RemoveAt(0);
            _alreadyVisited.Add(new IntVector3(current.X, current.Y));

            for (int i = 0; i < Directions.Length; i++) {
                int dirX = Directions[i].x;
                int dirY = Directions[i].y;
                int neighborX = current.X + Directions[i].x;
                int neighborY = current.Y + Directions[i].y;

                int distanceFromStart = Mapservice.DistanceFromStart(start.x, start.y, neighborX, neighborY);
                if (distanceFromStart > radius) {
                    continue;
                }

                if (_alreadyVisited.Contains(new IntVector3(neighborX, neighborY))) {
                    continue;
                }

                ITileInfo info = LevelDataManager.Instance.GetTileAt(neighborX, neighborY);
                bool _canTraverse = info != null && !info.Data.IsSolid;

                if (!_canTraverse || ContainsNode(neighborX, neighborY)) {
                    continue;
                }

                TileNode newNode = new TileNode() {
                    X = neighborX,
                    Y = neighborY,
                    DistanceFromStart = distanceFromStart
                };
                _toBeVisited.Add(newNode);
            }
            if (count > NumTriesAbort) {
                CustomLogger.Error(nameof(MapService), $"{nameof(GetTraversableTiles)} Aborting after {count} steps!");
                break;
            }
        }

        return traversableTargets;
    }

    public static List<IntVector3> GetTraversableTiles(int radius, IntVector3 start, int minDistance = 0) {
        _toBeVisited.Clear();
        _alreadyVisited.Clear();

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

        _toBeVisited.Add(current);
        int count = 0;

        while(_toBeVisited.Count != 0) {
            count++;
            current = _toBeVisited[0];
            TryAddToList(minDistance, traversableTargets, current);
            _toBeVisited.RemoveAt(0);
            _alreadyVisited.Add(new IntVector3(current.X, current.Y));

            for(int i = 0; i < Directions.Length; i++) {
                int dirX = Directions[i].x;
                int dirY = Directions[i].y;
                int neighborX = current.X + Directions[i].x;
                int neighborY = current.Y + Directions[i].y;

                int distanceFromStart = Mapservice.DistanceFromStart(start.x, start.y, neighborX, neighborY);
                if(distanceFromStart > radius) {
                    continue;
                }

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
                    if (neighborTileX != null) {
                        _canTraverse &= !neighborTileX.Data.IsSolid;
                    }
                    if(neighborTileY != null) {
                        _canTraverse &= !neighborTileY.Data.IsSolid;
                    }
                }

                if (ContainsNode(neighborX, neighborY)) {
                    continue;
                }

                TileNode newNode = new TileNode() {
                    X = neighborX,
                    Y = neighborY,
                    DistanceFromStart = distanceFromStart
                };
                _toBeVisited.Add(newNode);
            }
            if(count > NumTriesAbort) {
                CustomLogger.Error(nameof(MapService), $"{nameof(GetTraversableTiles)} Aborting after {count} steps!");
                break;
            }
        }

        return traversableTargets;
    }

    public static void TryAddToList(int minDistance, List<IntVector3> list, TileNode node) {
        if(minDistance <= 0) {
            list.Add(new IntVector3(node.X, node.Y));
            return;
        }
        if(node.DistanceFromStart >= minDistance) {
            list.Add(new IntVector3(node.X, node.Y));
        }
    }

    private static bool ContainsNode(int x, int y) {
        for (int i = 0; i < _toBeVisited.Count; i++) {
            if (_toBeVisited[i].X == x && _toBeVisited[i].Y == y) {
                return true;
            }
        }
        return false;
    }
}
