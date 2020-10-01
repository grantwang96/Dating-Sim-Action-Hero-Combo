using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTile : MonoBehaviour, ITileOccupant
{
    [SerializeField] private string _tileType;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private IntVector3 _mapPosition;

    private void Start() {
        RegisterTile();
    }

    private void OnDisable() {
        UnregisterTile();
    }

    private void OnEnable() {
        if(LevelDataManager.Instance == null) {
            return;
        }
        RegisterTile();
    }

    private void RegisterTile() {
        _mapPosition = LevelDataManager.Instance.WorldToArraySpace(transform.position);
        LevelDataManager.Instance.UpdateTile(_mapPosition.x, _mapPosition.y, _tileType);
        LevelDataManager.Instance.AddOccupant(_mapPosition, this);
    }

    private void InitializeTile() {
        ITileInfo info = LevelDataManager.Instance.GetTileAt(_mapPosition.x, _mapPosition.y);
        _collider.enabled = info.Data.IsSolid;
    }

    private void UnregisterTile() {
        LevelDataManager.Instance.UpdateTile(_mapPosition.x, _mapPosition.y);
        LevelDataManager.Instance.RemoveOccupant(_mapPosition, this);
    }
}