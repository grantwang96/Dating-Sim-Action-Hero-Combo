using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTile : MonoBehaviour
{
    [SerializeField] private string _tileType;
    [SerializeField] private Collider _collider;

    private IntVector3 _mapPosition;

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
    }

    private void InitializeTile() {
        ITileInfo info = LevelDataManager.Instance.GetTileAt(_mapPosition.x, _mapPosition.y);
        _collider.enabled = info.Data.IsSolid;
    }

    private void UnregisterTile() {
        LevelDataManager.Instance.UpdateTile(_mapPosition.x, _mapPosition.y);
    }
}