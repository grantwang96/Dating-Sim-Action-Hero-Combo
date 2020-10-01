﻿using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that helps build paths for NPCs and enemies
/// </summary>

public partial class MapService
{

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

    public static PathStatus GetPathToDestination(IntVector3 startPosition,
        IntVector3 targetDestination, 
       List<IntVector3> path,
      ITileOccupant occupant,
      int traversableThreshold) {
        // setup for path finding
        path?.Clear();
        List<TileNode> toBeVisited = new List<TileNode>();
        List<IntVector3> alreadyVisited = new List<IntVector3>();

        // validate the given destination first
        if (!LevelDataManager.Instance.IsWithinMap(targetDestination)) {
            CustomLogger.Warn(nameof(MapService), $"Target Destination {targetDestination} is out of bounds!");
            return PathStatus.Invalid;
        }
        ITileInfo startingTileInfo = LevelDataManager.Instance.GetTileAt(targetDestination.x, targetDestination.y);
        if (startingTileInfo != null && (startingTileInfo.Occupants.Count > traversableThreshold || startingTileInfo.Data.IsSolid)) {
            CustomLogger.Warn(nameof(MapService), $"Target Destination {targetDestination} Invalid!");
            return PathStatus.Invalid;
        }

        int startX = startPosition.x;
        int startY = startPosition.y;
        int targetX = targetDestination.x;
        int targetY = targetDestination.y;
        toBeVisited.Clear();
        alreadyVisited.Clear();

        // this current node will be the first node that we check
        TileNode current = new TileNode() {
            X = startX,
            Y = startY,
            DistanceFromStart = 0,
            TotalCost = 0
        };
        toBeVisited.Add(current);
        int count = 0;
        while(toBeVisited.Count != 0) {
            // get current node
            count++;
            current = GetLowestCostNode(toBeVisited);
            // remove from to be visited
            toBeVisited.Remove(current);

            // if we've hit our destination
            if (current.X == targetX && current.Y == targetY) {
                while(current.Parent != null) {
                    path.Insert(0, new IntVector3(current.X, current.Y));
                    current = current.Parent;
                }
                return PathStatus.Complete;
            }

            // checks all directions for valid, unexplored tiles
            for(int i = 0; i < _directions.Length; i++) {
                int dirX =_directions[i].x;
                int dirY = _directions[i].y;
                int neighborX = current.X + dirX;
                int neighborY = current.Y + dirY;
                IntVector3 neighbor = new IntVector3(neighborX, neighborY);
                // check if we've already looked here
                if (ContainsIntVector3(neighborX, neighborY, alreadyVisited)) {
                    continue;
                }
                // check if this tile is on the map
                if(!LevelDataManager.Instance.IsWithinMap(neighbor)) {
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
                // if this tile is not traversable, ignore it
                if (!_canTraverse) {
                    continue;
                }

                int distanceFromStart = DistanceFromStart(startX, startY, neighborX, neighborY);

                // if this node is in "to be visited", check to see if the distance value needs to be updated and skipped
                bool markedToBeVisited = TryGetNode(neighborX, neighborY, toBeVisited, out int toBeVisitedIndex);
                if (markedToBeVisited) {
                    TileNode toBeVisitedNode = toBeVisited[toBeVisitedIndex];
                    if (distanceFromStart < toBeVisitedNode.DistanceFromStart) {
                        toBeVisitedNode.DistanceFromStart = distanceFromStart;
                    }
                    // do not add again to the list
                    continue;
                }

                // create a new node and add to open list
                TileNode newNode = new TileNode() {
                    X = neighborX,
                    Y = neighborY,
                    TotalCost = GetNodeTotalCost(startX, startY, neighborX, neighborY, targetX, targetY),
                    DistanceFromStart = distanceFromStart,
                    Parent = current
                };
                toBeVisited.Add(newNode);
            }
            alreadyVisited.Add(new IntVector3(current.X, current.Y));

            if (count > NumTriesAbort) {
                CustomLogger.Error(nameof(MapService), $"In \"{nameof(GetPathToDestination)}\" Failed to path find to {targetDestination}! Aborting...");
                break;
            }
        }
        return PathStatus.Invalid;
    }

    private static bool IsTileTraversable(ITileInfo tileInfo, ITileOccupant occupant, int traversableThreshold) {
        bool isTraversable = tileInfo != null;
        isTraversable |= traversableThreshold > tileInfo.Occupants.Count;
        isTraversable |= tileInfo.ContainsOccupant(occupant);
        isTraversable &= !tileInfo.Data.IsSolid;
        return isTraversable;
    }

    private static bool TryGetNode(int x, int y, List<TileNode> toBeVisited, out int index) {
        for(int i = 0; i < toBeVisited.Count; i++) {
            if(toBeVisited[i].X == x && toBeVisited[i].Y == y) {
                index = i;
                return true;
            }
        }
        index = -1;
        return false;
    }

    private static TileNode GetLowestCostNode(List<TileNode> toBeVisited) {
        if(toBeVisited.Count == 0) {
            return null;
        }
        TileNode lowestCostNode = toBeVisited[0];
        for(int i = 1; i < toBeVisited.Count; i++) {
            if(toBeVisited[i].TotalCost < lowestCostNode.TotalCost) {
                lowestCostNode = toBeVisited[i];
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
