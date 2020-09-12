using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface ILevelDataManager : IInitializableManager {

    int MapBoundsX { get; }
    int MapBoundsY { get; }

    IntVector3 WorldToArraySpace(Vector2 worldPos);
    Vector2 ArrayToWorldSpace(int x, int y);

    bool IsWithinMap(IntVector3 position);
    void UpdateTile(int x, int y, string tileType = "");
    ITileInfo GetTileAt(int x, int y);
    List<ITileInfo> GetTilesWithinRadius(IntVector3 position, int radius);
    void SetOccupant(IntVector3 position, ITileOccupant occupant);

    bool TryGetEnemySpawn(string id, out EnemySpawn spawn);
    void RegisterEnemySpawn(string id, EnemySpawn spawn);
    void DeregisterEnemySpawn(string id);
}

public enum PathStatus {
    Invalid,
    Partial,
    Complete
}

public class LevelDataManager : ILevelDataManager {

    public static ILevelDataManager Instance { get; private set; }
    
    public int MapBoundsX { get; private set; }
    public int MapBoundsY { get; private set; }

    private Dictionary<string, EnemySpawn> _enemySpawnPoints = new Dictionary<string, EnemySpawn>();

    private ITileInfo[][] _tiles;
    private Dictionary<string, TileData> _tileConfig = new Dictionary<string, TileData>();

    public void Initialize(Action<bool> initializationCallback = null) {
        Instance = this;
        LoadTileConfig();
        InitializeMap();
        initializationCallback?.Invoke(true);
    }

    public void Dispose() {

    }

    private void LoadTileConfig() {
        _tileConfig.Clear();
        GameLevelData gameLevel = GameLevelDataController.Instance.CurrentGameLevelData;
        for(int i = 0; i < gameLevel.MapData.TileDatas.Count; i++) {
            _tileConfig.Add(gameLevel.MapData.TileDatas[i].name, gameLevel.MapData.TileDatas[i]);
        }
    }

    private void InitializeMap() {
        MapBoundsX = GameLevelDataController.Instance.CurrentGameLevelData.MapData.MapSizeX;
        MapBoundsY = GameLevelDataController.Instance.CurrentGameLevelData.MapData.MapSizeY;
        _tiles = new ITileInfo[MapBoundsX][];
        for (int x = 0; x < _tiles.Length; x++) {
            _tiles[x] = new ITileInfo[MapBoundsY];
            for (int y = 0; y < _tiles[x].Length; y++) {
                _tiles[x][y] = new TileInfo(x, y, GameLevelDataController.Instance.CurrentGameLevelData.MapData.DefaultTileData);
            }
        }
    }

    public void UpdateTile(int x, int y, string tileType = "") {
        TileData data;
        if (string.IsNullOrEmpty(tileType)) {
            data = GameLevelDataController.Instance.CurrentGameLevelData.MapData.DefaultTileData;
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

    public List<ITileInfo> GetTilesWithinRadius(IntVector3 position, int radius) {
        List<ITileInfo> tilesList = new List<ITileInfo>();
        if (!IsWithinMap(position)) {
            CustomLogger.Warn(nameof(LevelDataManager), $"Position {position} is out of bounds!");
            return tilesList;
        }
        List<IntVector3> adjacentPositions = MapService.GetPositionsWithinRadius(0, position, radius);
        for(int i = 0; i < adjacentPositions.Count; i++) {
            if (IsWithinMap(adjacentPositions[i])) {
                tilesList.Add(_tiles[adjacentPositions[i].x][adjacentPositions[i].y]);
            }
        }
        return tilesList;
    }

    public bool IsWithinMap(IntVector3 position) {
        return position.x >= 0 && position.x < MapBoundsX && position.y >= 0 && position.y < MapBoundsY;
    }

    public void SetOccupant(IntVector3 position, ITileOccupant occupant) {
        if (!IsWithinMap(position)) {
            CustomLogger.Error(nameof(LevelDataManager), $"Position {position} is out of bounds!");
            return;
        }
        _tiles[position.x][position.y].SetOccupant(occupant);
    }

    public IntVector3 WorldToArraySpace(Vector2 worldPos) {
        IntVector3 mapPosition = new IntVector3();
        mapPosition.x = Mathf.RoundToInt(worldPos.x + (MapBoundsX / 2f));
        mapPosition.y = Mathf.RoundToInt(worldPos.y + (MapBoundsY / 2f));
        return mapPosition;
    }

    public Vector2 ArrayToWorldSpace(int x, int y) {
        Vector2 worldPosition = new Vector2();
        worldPosition.x = x - (MapBoundsX / 2f);
        worldPosition.y = y - (MapBoundsY / 2f);
        return worldPosition;
    }

    public bool TryGetEnemySpawn(string id, out EnemySpawn spawn) {
        return _enemySpawnPoints.TryGetValue(id, out spawn);
    }

    public void RegisterEnemySpawn(string id, EnemySpawn spawn) {
        if (_enemySpawnPoints.ContainsKey(id)) {
            return;
        }
        _enemySpawnPoints.Add(id, spawn);
    }

    public void DeregisterEnemySpawn(string id) {
        _enemySpawnPoints.Remove(id);
    }
}
