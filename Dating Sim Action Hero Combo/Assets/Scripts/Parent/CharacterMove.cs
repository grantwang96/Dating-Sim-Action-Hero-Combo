using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The base class for all objects that intelligiently move around the map
/// </summary>
public abstract class CharacterMove : MonoBehaviour {

    [SerializeField] protected Rigidbody2D rbody;
    protected Stack<Vector2> path = new Stack<Vector2>();
    private Vector2 _currentDestination;
    public Vector2 currentDestination { get { return _currentDestination; } }

    [SerializeField] protected float _walkSpeed;
    public float walkSpeed { get { return _walkSpeed; } }
    [SerializeField] protected float _runSpeed;
    public float runSpeed { get { return _runSpeed; } }

    private Coroutine _movementRoutine;
    public Coroutine movementRoutine { get { return _movementRoutine; } }

    [SerializeField] private GameObject markerPrefab;
    private Damageable damageable;

    protected virtual void Awake() {
        rbody = GetComponent<Rigidbody2D>();
        damageable = GetComponent<Damageable>();
    }

    protected virtual void Start() {

    }

    protected virtual void Update() {
        
    }

    // this sets the destination of the character and calls the CalculatePath coroutine
    public void SetDestination(int newX, int newY) {
        // if (GameManager.Instance.LocationOccupied(newDestination)) { Debug.Log("Blocked!"); return; }
        if(GameManager.Instance.grid[newX, newY] != null) { Debug.Log("Blocked!"); return; }
        if(!GameManager.Instance.IsWithinGridSpace(newX, newY)) { return; }
        if (_movementRoutine != null) { StopCoroutine(_movementRoutine); }

        // Calculate path
        _movementRoutine = StartCoroutine(CalculatePath(newX, newY));
    }

    // this makes the character wander a random path
    public void SetDestination(int steps) {
        if (_movementRoutine != null) { StopCoroutine(_movementRoutine); }
        _movementRoutine = StartCoroutine(CalculatePathRandom(steps));
    }

    // this initiates the search for a point with the best cover
    public void SetDestination(Vector2 desiredCover) {
        if (_movementRoutine != null) { StopCoroutine(_movementRoutine); }
        _movementRoutine = StartCoroutine(CalculatePathCover(desiredCover));
    }

    // this calls the TravelProcess coroutine to move the player, if it has a path
    public void MoveToDestination(float speed) {
        if(path.Count == 0) { return; }
        if (_movementRoutine != null) { StopCoroutine(_movementRoutine); }

        _movementRoutine = StartCoroutine(TravelProcess(speed));
    }

    // cancels the current path and current movement
    public void CancelDestination() {
        if(movementRoutine != null) { StopCoroutine(_movementRoutine); }
        path.Clear();
        _movementRoutine = null;
    }

    // this hard sets the rotation of the character
    public void SetRotation(Vector2 dir) {
        transform.up = dir;
    }

    // gets a path to a specific point on the grid
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
                GeneratePath(current);
                break; // leave this while loop
            }

            // check every direction for possible movement
            foreach (Vector2 dir in dirs) {
                int newX = current.xPosition + Mathf.RoundToInt(dir.x);
                int newY = current.yPosition + Mathf.RoundToInt(dir.y);

                if (newX >= GameManager.Instance.mapWidth || newX < 0 || 
                    newY >= GameManager.Instance.mapHeight || newY < 0 ||
                    GameManager.Instance.grid[newX, newY] != null || beenThere.Contains(new Vector2(newX, newY))) { continue; } // skip this location if it's blocked or we've been there

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
        Debug.Log("Calculating random path for " + name + "...");
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
            beenThere.Add(new Vector2(current.xPosition, current.yPosition));

            // you've found a new position
            if (current.distanceFromStart == steps) {
                Debug.Log("Found destination!");
                GeneratePath(current);
                break; // leave this while loop
            }

            // check every direction for possible movement

            for(int i = 0; i < dirs.Length; i++) {
                Vector2 temp = dirs[i];
                int rand = Random.Range(0, dirs.Length);
                dirs[i] = dirs[rand];
                dirs[rand] = temp;
            }

            foreach (Vector2 dir in dirs) {
                int newX = current.xPosition + Mathf.RoundToInt(dir.x);
                int newY = current.yPosition + Mathf.RoundToInt(dir.y);

                if (newX >= GameManager.Instance.mapWidth || newX < 0 ||
                    newY >= GameManager.Instance.mapHeight || newY < 0 ||
                    GameManager.Instance.grid[newX, newY] != null || beenThere.Contains(new Vector2(newX, newY))) { continue; } // skip this location if it's blocked or we've been there

                Vertex newVertex = Vertex.GenerateVertex(toBeVisited, newX, newY);
                newVertex.xPosition = newX;
                newVertex.yPosition = newY;
                newVertex.manhattanDistanceToDestination = 0;
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

    // finds the nearest point of cover on the map based on desired direction
    protected IEnumerator CalculatePathCover(Vector2 desiredCover) {
        Debug.Log("Calculating cover path for " + name + "...");
        path.Clear(); // clear our current path (if we have one)
        List<Vertex> toBeVisited = new List<Vertex>(); // queue for vertices that we have yet to visit
        List<Vertex> beenThere = new List<Vertex>(); // list to make sure we don't repeat locations

        Vector2[] dirs = { Vector2.left, Vector2.right, Vector2.up, Vector2.down };

        Vertex current = new Vertex();

        // our starting position is our current position;
        current.xPosition = damageable.XPos;
        current.yPosition = damageable.YPos;
        current.manhattanDistanceToDestination = 0; // we have no destination. so it doesn't matter
        current.distanceFromStart = 0; // calculate distance from start (on path)
        current.GenerateCoverScore(desiredCover);
        toBeVisited.Add(current); // add to "to be visited"

        // while not all possibilities haven't been expended yet
        while (toBeVisited.Count > 0) {

            current = toBeVisited[0];
            toBeVisited.Remove(current);
            beenThere.Add(current);

            // you've found a new position
            if (current.coverScore >= 2) {
                Debug.Log("Found destination!");
                GeneratePath(current);
                break; // leave this while loop
            }

            // check every direction for possible movement
            
            foreach (Vector2 dir in dirs) {
                int newX = current.xPosition + Mathf.RoundToInt(dir.x);
                int newY = current.yPosition + Mathf.RoundToInt(dir.y);

                if (newX >= GameManager.Instance.mapWidth || newX < 0 ||
                    newY >= GameManager.Instance.mapHeight || newY < 0 ||
                    GameManager.Instance.grid[newX, newY] != null) { continue; } // skip this location if it's blocked

                Vertex newVertex = Vertex.GenerateVertex(toBeVisited, newX, newY);
                if (beenThere.Contains(newVertex)) { continue; } // skip this location if we've been there
                newVertex.xPosition = newX;
                newVertex.yPosition = newY;
                newVertex.manhattanDistanceToDestination = 0;
                newVertex.distanceFromStart = current.distanceFromStart + 1;
                newVertex.distanceScore = newVertex.manhattanDistanceToDestination + newVertex.distanceFromStart;
                newVertex.GenerateCoverScore(desiredCover);
                newVertex.previous = current;

                if (!toBeVisited.Contains(newVertex)) { toBeVisited.Add(newVertex); }
            }

            // Vertex.Split(toBeVisited, 0, toBeVisited.Count - 1); // sort list by lowest distance score(faux priority queue)
            yield return new WaitForSeconds(.01f);
        }

        // if we did not find perfect cover
        if(path.Count == 0) {
            for(int i = 0; i < beenThere.Count; i++) {
                if (beenThere[i].coverScore == 1) { GeneratePath(beenThere[i]); break; }
            }
        }

        Debug.Log("Finished calculating path");
        _movementRoutine = null;
    }

    // Generates the path to the vertex location
    protected void GeneratePath(Vertex point) {
        while (point.previous != null) {
            path.Push(new Vector2(point.xPosition, point.yPosition)); // add current position to the stack
            point = point.previous; // backtrack to previous position, this means the first object in stack is the starting point
        }
    }

    // where we move the character
    protected IEnumerator TravelProcess(float speed) {
        while (path.Count != 0) {
            Vector2 gridDest = path.Peek();
            int newX = Mathf.RoundToInt(gridDest.x);
            int newY = Mathf.RoundToInt(gridDest.y);

            // the space has been occupied by something else
            if(GameManager.Instance.grid[newX, newY] != null) {
                path.Clear();
                break;
            }

            _currentDestination = GameManager.GetWorldSpace(newX, newY);
            damageable.SetPosition(newX, newY); // update current grid position
            while (Vector2.Distance(transform.position, currentDestination) > 0.1f) {
                rbody.MovePosition(rbody.position + (currentDestination - rbody.position).normalized * Time.deltaTime * speed);
                yield return null;
            }
            path.Pop();
            yield return null;
        }
        _movementRoutine = null;
    }

    // this returns the integer position of the character
    public static Vector2 GetRoundedPosition(Vector2 position) {
        return new Vector2(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
    }

    // return the raw grid distance of two locations
    public static int GetManhattanDistance(int startX, int startY, int endX, int endY) {
        return Mathf.Abs(startX - endX) + Mathf.Abs(startY - endY);
    }
    
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
    public int coverScore; // 0 = no cover, 1 = ok cover, 2 = best cover
    
    // returns the vertex with the smallest distance
    public static Vertex GetSmallestDistanceScore(List<Vertex> vertices)
    {
        Vertex smallest = vertices[0];
        for (int i = 0; i < vertices.Count; i++)
        {
            if (vertices[i].distanceScore < smallest.distanceScore) { smallest = vertices[i]; }
        }
        return smallest;
    }

    // creates a vertex object a certain point. Updates a vertex if one already exists at that location
    public static Vertex GenerateVertex(List<Vertex> vertices, int xPos, int yPos) {
        for (int i = 0; i < vertices.Count; i++) {
            if (vertices[i].xPosition == xPos && vertices[i].yPosition == yPos) {
                return vertices[i];
            }
        }
        return new Vertex();
    }

    // split portion of merge sort
    public static void Split(List<Vertex> list, int l, int r)
    {
        if (l == r) { return; }
        int middle = (l + r) / 2;
        Split(list, l, middle);
        Split(list, middle + 1, r);
        Merge(list, l, middle, r);
    }

    // merge portion of merge sort
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

    // returns the cover value of this location
    public void GenerateCoverScore(Vector2 desiredCover) {

        coverScore = 0; // be sure to preset this to 0

        int x = Mathf.RoundToInt(desiredCover.x);
        int y = Mathf.RoundToInt(desiredCover.y);

        if(x != 0 && GameManager.Instance.IsWithinGridSpace(xPosition + x, yPosition) && GameManager.Instance.grid[xPosition + x, yPosition]) {
            coverScore += (y == 0) ? 2 : 1; // add 2 if y cover is not necessary
        }

        if(y != 0 && GameManager.Instance.IsWithinGridSpace(xPosition, yPosition + y) && GameManager.Instance.grid[xPosition, yPosition + y]) {
            coverScore += (x == 0) ? 2 : 1; // add 2 if x cover is not necessary
        }
        Debug.Log(xPosition + ", " + yPosition + " score: " + coverScore);
    }
}
