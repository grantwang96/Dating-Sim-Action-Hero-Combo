using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivilianMovement : CharacterMove {

    protected override IEnumerator CalculatePathCover(Vector2 desiredCover, int range) {
        Debug.Log("Calculating cover path for " + name + "...");
        path.Clear(); // clear our current path (if we have one)

        Vertex next;

        int newXDir = Mathf.RoundToInt(desiredCover.x);
        int newYDir = Mathf.RoundToInt(desiredCover.y);

        Vertex current = new Vertex();

        // our starting position is our current position;
        current.xPosition = damageable.XPos;
        current.yPosition = damageable.YPos;
        current.distanceFromStart = 0;
        next = current;

        // while not all possibilities haven't been expended yet
        while (next != null) {

            current = next;
            next = null;

            int newX = current.xPosition + newXDir;
            int newY = current.yPosition + newYDir;

            if(newX == current.xPosition && newY == current.yPosition) {
                break;
            }

            Vertex newVertex = new Vertex();
            newVertex.xPosition = -1;

            if(GameManager.Instance.IsWithinGridSpace(newX, newY)) {
                if(GameManager.Instance.grid[newX, newY] == null) {
                    newVertex = CreateVertex(newX, newY, current.distanceFromStart + 1);
                } else if(GameManager.Instance.grid[newX, current.yPosition] == null && newX != current.xPosition) {
                    newVertex = CreateVertex(newX, current.yPosition, current.distanceFromStart + 1);
                } else if(GameManager.Instance.grid[current.xPosition, newY] == null && newY != current.yPosition) {
                    newVertex = CreateVertex(current.xPosition, newY, current.distanceFromStart + 1);
                }
            }
            if(newVertex.xPosition == -1) { break; }
            newVertex.previous = current;
            next = newVertex;
            yield return null;
        }

        GeneratePath(current);
        Debug.Log(path.Count);

        Debug.Log("Finished calculating path");
        _movementRoutine = null;
    }

    private Vertex CreateVertex(int newX, int newY, int dist) {
        Vertex v = new Vertex();
        v.xPosition = newX;
        v.yPosition = newY;
        v.distanceFromStart = dist;
        return v;
    }
}
