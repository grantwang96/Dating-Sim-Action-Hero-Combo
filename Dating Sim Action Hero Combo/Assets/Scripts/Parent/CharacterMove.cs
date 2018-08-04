﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The base class for all objects that intelligiently move around the map
/// </summary>
public abstract class CharacterMove : MonoBehaviour {

    [SerializeField] protected Rigidbody2D rbody;
    protected Stack<Vector2> path = new Stack<Vector2>();

    [SerializeField] protected float _speed;
    public float speed { get { return _speed; } }

    private Coroutine _movementRoutine;
    public Coroutine movementRoutine { get { return _movementRoutine; } }

    [SerializeField] private GameObject markerPrefab;
    private Damageable damageable;

    protected virtual void Start() {
        rbody = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        
    }

    // this sets the destination of the character and calls the CalculatePath coroutine
    public void SetDestination(int newX, int newY) {
        // if (GameManager.Instance.LocationOccupied(newDestination)) { Debug.Log("Blocked!"); return; }
        if(GameManager.Instance.grid[newX, newY] != null) { Debug.Log("Blocked!"); return; }
        if(newX >= GameManager.Instance.mapWidth || newX < 0 || newY >= GameManager.Instance.mapHeight || newY < 0) { return; }
        if (_movementRoutine != null) { StopCoroutine(_movementRoutine); }

        // Calculate path
        _movementRoutine = StartCoroutine(CalculatePath(newX, newY));
    }

    // this makes the character wander a random path
    public void SetDestination(int steps) {
        if (_movementRoutine != null) { StopCoroutine(_movementRoutine); }
        _movementRoutine = StartCoroutine(CalculatePathRandom(steps));
    }

    // this calls the TravelProcess coroutine to move the player, if it has a path
    public void MoveToDestination() {
        if(path.Count == 0) { return; }
        if (_movementRoutine != null) { StopCoroutine(_movementRoutine); }

        _movementRoutine = StartCoroutine(TravelProcess());
    }

    // this hard sets the rotation of the character
    public void SetRotation(Vector2 dir) {
        if (_movementRoutine == null) { transform.up = dir; }
    }

    // this coroutine will figure out a path for the character
    protected IEnumerator CalculatePath(int xDest, int yDest) {
        Debug.Log("Calculating path for " + name + "...");
        path.Clear(); // clear our current path (if we have one)
        List<Vertex> toBeVisited = new List<Vertex>(); // queue for vertices that we have yet to visit
        List<Vector2> beenThere = new List<Vector2>(); // list to make sure we don't repeat locations

        Vector2[] dirs = { Vector2.left, Vector2.right, Vector2.up, Vector2.down };

        Vertex current = new Vertex();
        current.xPosition = damageable.XPos;
        current.yPosition = damageable.YPos;
        current.manhattanDistanceToDestination =
            GetManhattanDistance(current.xPosition, current.yPosition, xDest, yDest); // calculate raw distance from goal
        current.distanceFromStart = 0; // calculate distance from start (on path)
        current.distanceScore = current.distanceFromStart + current.manhattanDistanceToDestination; // generate total score
        toBeVisited.Add(current); // add to "to be visited"

        // while not all possibilities haven't been expended yet
        while (toBeVisited.Count > 0) {
            current = Vertex.GetSmallestDistanceScore(toBeVisited);
            toBeVisited.Remove(current);
            beenThere.Add(new Vector2(current.xPosition, current.yPosition));

            // you've found your destination
            if (current.xPosition == xDest && current.yPosition == yDest) { 
                Debug.Log("Found destination!");
                while (current.previous != null) {
                    path.Push(GameManager.GetWorldSpace(current.xPosition, current.yPosition)); // add current position to the stack
                    current = current.previous; // backtrack to previous position, this means the first object in stack is the starting point
                    yield return null;
                }
                break; // leave this while loop
            }

            // check every direction for possible movement
            foreach (Vector2 dir in dirs) {
                int newX = current.xPosition + Mathf.RoundToInt(dir.x);
                int newY = current.yPosition + Mathf.RoundToInt(dir.y);
                if (GameManager.Instance.grid[newX, newY] != null || beenThere.Contains(new Vector2(newX, newY))) { continue; } // skip this location if it's blocked or we've been there

                Vertex newVertex = Vertex.GenerateVertex(toBeVisited, newX, newY);
                newVertex.xPosition = newX;
                newVertex.yPosition = newY;
                newVertex.manhattanDistanceToDestination = GetManhattanDistance(newX, newY, xDest, yDest);
                newVertex.distanceFromStart = current.distanceFromStart + 1;
                newVertex.distanceScore = newVertex.manhattanDistanceToDestination + newVertex.distanceFromStart;
                newVertex.previous = current;

                if (!toBeVisited.Contains(newVertex)) { toBeVisited.Add(newVertex); }
            }

            // Vertex.Split(toBeVisited, 0, toBeVisited.Count - 1); // sort list by lowest distance score(faux priority queue)
            yield return new WaitForSeconds(.01f);
        }
        _movementRoutine = null;
    }

    // gets a random location from the starting point. Used for wandering
    protected IEnumerator CalculatePathRandom(int steps) {
        Debug.Log("Calculating path for " + name + "...");
        path.Clear(); // clear our current path (if we have one)
        List<Vertex> toBeVisited = new List<Vertex>(); // queue for vertices that we have yet to visit
        List<Vector2> beenThere = new List<Vector2>(); // list to make sure we don't repeat locations

        Vector2[] dirs = { Vector2.left, Vector2.right, Vector2.up, Vector2.down };

        Vertex current = new Vertex();

        // our starting position is our current position;
        current.xPosition = damageable.XPos; 
        current.yPosition = damageable.YPos;
        current.manhattanDistanceToDestination = 0; // we have no destination. so it doesn't matter
        current.distanceFromStart = 0; // calculate distance from start (on path)
        toBeVisited.Add(current); // add to "to be visited"

        // while not all possibilities haven't been expended yet
        while (toBeVisited.Count > 0) {
            // current = Vertex.GetSmallestDistanceScore(toBeVisited);
            current = toBeVisited[0];
            toBeVisited.Remove(current);
            beenThere.Add(current.position);

            // you've found a new position
            if (current.distanceFromStart == steps) {
                Debug.Log("Found destination!");
                while (current.previous != null) {
                    path.Push(current.position); // add current position to the stack
                    current = current.previous; // backtrack to previous position, this means the first object in stack is the starting point
                    yield return null;
                }
                break; // leave this while loop
            }

            // check every direction for possible movement
            foreach (Vector2 dir in dirs) {
                Vector2 newLoc = GetRoundedPosition(current.position + dir);
                bool isObstructed = GameManager.Instance.obstructions.Contains(newLoc); // ignore this direction if path is blocked
                if (isObstructed || beenThere.Contains(newLoc)) { continue; } // skip this location if it's blocked or we've been there

                Vertex newVertex = Vertex.GenerateVertex(toBeVisited, newLoc);
                newVertex.position = newLoc;
                newVertex.manhattanDistanceToDestination = 0;
                newVertex.distanceFromStart = current.distanceFromStart + 1;
                newVertex.previous = current;

                if (!toBeVisited.Contains(newVertex)) { toBeVisited.Add(newVertex); }
            }

            // Vertex.Split(toBeVisited, 0, toBeVisited.Count - 1); // sort list by lowest distance score(faux priority queue)
            yield return new WaitForSeconds(.01f);
        }
        _movementRoutine = null;
    }

    // where we move the character
    protected IEnumerator TravelProcess() {

        while (path.Count != 0) {
            float time = 0f;
            Vector2 startPos = transform.position;
            Vector2 dest = path.Peek();
            while (time < 1f) {
                time += Time.deltaTime * speed;
                Vector2 curr = Vector2.Lerp(startPos, dest, time);
                // transform.up = Vector2.Lerp(transform.up, curr - (Vector2)transform.position, 0.75f);
                rbody.MovePosition(curr);
                yield return null;
            }
            path.Pop();
            yield return null;
        }
        _movementRoutine = null;
    }

    // this returns the integer position of the character
    public static Vector2 GetRoundedPosition(Vector2 position)
    {
        return new Vector2(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
    }

    // return the raw grid distance of two locations
    public static int GetManhattanDistance(int startX, int startY, int endX, int endY) {
        return Mathf.Abs(startX - endX) + Mathf.Abs(startY - endY);
    }

    /*
    protected static Vertex GetSmallestDistanceScore(List<Vertex> vertices) {
        Vertex smallest = vertices[0];
        for(int i = 0; i < vertices.Count; i++) {
            if(vertices[i].distanceScore < smallest.distanceScore) { smallest = vertices[i]; }
        }
        return smallest;
    }

    protected static Vertex GenerateVertex(List<Vertex> vertices, Vector2 location) {
        for(int i = 0; i < vertices.Count; i++) { if (vertices[i].position == location) { return vertices[i]; } }
        return new Vertex();
    }
    
    protected static void Split(List<Vertex> list, int l, int r) {
        if (l == r) { return; }
        int middle = (l + r) / 2;
        Split(list, l, middle);
        Split(list, middle + 1, r);
        Merge(list, l, middle, r);
    }

    protected static void Merge(List<Vertex> list, int l, int m, int r) {
        int i, j;
        Vertex[] lArr = new Vertex[m - l + 1];
        Vertex[] rArr = new Vertex[r - m];

        for (i = 0; i < lArr.Length; i++) { lArr[i] = list[i]; }
        for (j = 0; j < rArr.Length; j++) { rArr[j] = list[m + 1 + j]; }

        i = 0;
        j = 0;
        int k = 0;
        while (i < lArr.Length && j < rArr.Length)
        {
            if (lArr[i].distanceScore < rArr[j].distanceScore) {
                list[k] = lArr[i];
                i++;
            } else {
                list[k] = rArr[j];
                j++;
            }
            k++;
        }
        while (i < lArr.Length) { list[k] = lArr[i]; i++; k++; }
        while (j < rArr.Length) { list[k] = rArr[j]; j++; k++; }
    }
    */
}

/// <summary>
/// Stores data for character pathfinding. Also includes helper functions regarding vertices
/// </summary>
public class Vertex {
    public int xPosition;
    public int yPosition;
    public int distanceScore;
    public int manhattanDistanceToDestination;
    public int distanceFromStart;
    public Vertex previous;
    
    public static Vertex GetSmallestDistanceScore(List<Vertex> vertices)
    {
        Vertex smallest = vertices[0];
        for (int i = 0; i < vertices.Count; i++)
        {
            if (vertices[i].distanceScore < smallest.distanceScore) { smallest = vertices[i]; }
        }
        return smallest;
    }

    public static Vertex GenerateVertex(List<Vertex> vertices, int xPos, int yPos) {
        for (int i = 0; i < vertices.Count; i++) {
            if (vertices[i].xPosition == xPos && vertices[i].yPosition == yPos) {
                return vertices[i];
            }
        }
        return new Vertex();
    }

    public static void Split(List<Vertex> list, int l, int r)
    {
        if (l == r) { return; }
        int middle = (l + r) / 2;
        Split(list, l, middle);
        Split(list, middle + 1, r);
        Merge(list, l, middle, r);
    }

    public static void Merge(List<Vertex> list, int l, int m, int r)
    {
        int i, j;
        Vertex[] lArr = new Vertex[m - l + 1];
        Vertex[] rArr = new Vertex[r - m];

        for (i = 0; i < lArr.Length; i++) { lArr[i] = list[i]; }
        for (j = 0; j < rArr.Length; j++) { rArr[j] = list[m + 1 + j]; }

        i = 0;
        j = 0;
        int k = 0;
        while (i < lArr.Length && j < rArr.Length)
        {
            if (lArr[i].distanceScore < rArr[j].distanceScore)
            {
                list[k] = lArr[i];
                i++;
            }
            else
            {
                list[k] = rArr[j];
                j++;
            }
            k++;
        }
        while (i < lArr.Length) { list[k] = lArr[i]; i++; k++; }
        while (j < rArr.Length) { list[k] = rArr[j]; j++; k++; }
    }
}
