using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILevelDataManager {

    int MapBoundsX { get; }
    int MapBoundsY { get; }

    IntVector3 WorldToArraySpace(Vector2 worldPos);
    Vector2 ArrayToWorldSpace(int x, int y);

    bool IsWithinMap(IntVector3 position);
    void UpdateTile(int x, int y, string tileType = "");
    ITileInfo GetTileAt(int x, int y);
}

public enum PathStatus {
    Invalid,
    Partial,
    Complete
}

public class LevelDataManager : MonoBehaviour, ILevelDataManager {

    public static ILevelDataManager Instance { get; private set; }

    [SerializeField] private int _mapSizeX;
    [SerializeField] private int _mapSizeY;
    public int MapBoundsX => _mapSizeX;
    public int MapBoundsY => _mapSizeY;

    [SerializeField] private TileData _defaultTileData; // default tile (empty, probably)
    [SerializeField] private TileData[] _tileDatas; // tile datas to preload

    private ITileInfo[][] _tiles;
    private Dictionary<string, TileData> _tileConfig = new Dictionary<string, TileData>();

    private void Awake() {
        Instance = this;
        LoadTileConfig();
        InitializeMap();
    }

    private void LoadTileConfig() {
        for(int i = 0; i < _tileDatas.Length; i++) {
            _tileConfig.Add(_tileDatas[i].name, _tileDatas[i]);
        }
    }

    private void InitializeMap() {
        _tiles = new ITileInfo[_mapSizeX][];
        for (int x = 0; x < _tiles.Length; x++) {
            _tiles[x] = new ITileInfo[_mapSizeY];
            for (int y = 0; y < _tiles[x].Length; y++) {
                _tiles[x][y] = new TileInfo(x, y, _defaultTileData);
            }
        }
    }

    public void UpdateTile(int x, int y, string tileType = "") {
        TileData data;
        if (string.IsNullOrEmpty(tileType)) {
            data = _defaultTileData;
        } else if(!_tileConfig.TryGetValue(tileType, out data)) {
            return;
        }
        _tiles[x][y].UpdateTile(data);
    }
    
    public ITileInfo GetTileAt(int x, int y) {
        if(x >= _tiles.Length || x < 0 || y >= _tiles[x].Length || y < 0) {
            return null;
        }
        return _tiles[x][y];
    }

    public bool IsWithinMap(IntVector3 position) {
        return position.x >= 0 && position.x < _mapSizeX && position.y >= 0 && position.y < _mapSizeY;
    }

    public IntVector3 WorldToArraySpace(Vector2 worldPos) {
        IntVector3 mapPosition = new IntVector3();
        mapPosition.x = Mathf.RoundToInt(worldPos.x + (_mapSizeX / 2f));
        mapPosition.y = Mathf.RoundToInt(worldPos.y + (_mapSizeY / 2f));
        return mapPosition;
    }

    public Vector2 ArrayToWorldSpace(int x, int y) {
        Vector2 worldPosition = new Vector2();
        worldPosition.x = x - (_mapSizeX / 2f);
        worldPosition.y = y - (_mapSizeY / 2f);
        return worldPosition;
    }
}
