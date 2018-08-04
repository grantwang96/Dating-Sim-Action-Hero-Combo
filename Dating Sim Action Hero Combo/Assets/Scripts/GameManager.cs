using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    [SerializeField] private int _mapWidth; // half-width of the map
    public int mapWidth { get { return _mapWidth; } }
    [SerializeField] private int _mapHeight; // half-height of the map
    public int mapHeight { get { return _mapHeight; } }

    public List<Damageable> entities = new List<Damageable>();
    public Damageable[,] grid;

	// Use this for initialization
	void Awake () {
        Instance = this;
        grid = new Damageable[_mapWidth, _mapHeight];
    }

    private void Start() {
        
    }

    // Update is called once per frame
    void Update () {
		
	}
    
    // returns whether an entity is occupying this space
    public bool LocationOccupied(Vector2 location) {
        for(int i = 0; i < entities.Count; i++) {
            Vector2 pos = CharacterMove.GetRoundedPosition(entities[0].transform.position);
            if(pos == location) { return true; }
        }
        return false;
    }

    // returns the location from grid space to world space
    public static Vector2 GetWorldSpace(int x, int y) {
        return new Vector2(x - (_mapWidth / 2), y - (_mapHeight / 2));
    }
}
