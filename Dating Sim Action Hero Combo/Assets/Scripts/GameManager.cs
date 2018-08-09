using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    [SerializeField] private int _mapWidth; // half-width of the map
    public int mapWidth { get { return _mapWidth; } }
    [SerializeField] private int _mapHeight; // half-height of the map
    public int mapHeight { get { return _mapHeight; } }
    
    public Damageable[,] grid; // holds the map data

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
    
    // returns the location from grid space to world space
    public static Vector2 GetWorldSpace(int x, int y) {
        return new Vector2(x - (Instance._mapWidth / 2), y - (Instance._mapHeight / 2));
    }

    public static int GetGridSpaceX(float x) { return Mathf.RoundToInt(x) + (Instance.mapWidth / 2); }

    public static int GetGridSpaceY(float y) { return Mathf.RoundToInt(y) + (Instance.mapHeight / 2); }
}
