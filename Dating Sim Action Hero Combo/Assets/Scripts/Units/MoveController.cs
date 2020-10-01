using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class MoveController : MonoBehaviour, IUnitComponent {

    [SerializeField] protected Rigidbody2D _rigidbody;
    [SerializeField] protected Transform _body;
    [SerializeField] protected Transform _front;
    [SerializeField] protected IntVector3 _mapPosition;
    [SerializeField] protected Unit _unit;

    public IntVector3 MapPosition => _mapPosition;
    public Transform Body => _body;
    public Transform Front => _front;

    protected bool _active = true;

    protected abstract void ProcessMovement();
    protected abstract void ProcessRotation();

    public event Action<IntVector3> OnMapPositionUpdated;

    public virtual void Initialize() {
        UpdateMapSpacePosition(LevelDataManager.Instance.WorldToArraySpace(transform.position));
        _active = true;
        GameEventsManager.PauseMenu.Subscribe(OnGamePause);
    }

    public virtual void Dispose() {
        GameEventsManager.PauseMenu.Unsubscribe(OnGamePause);
    }

    protected virtual void FixedUpdate() {
        if (!_active) {
            return;
        }
        ProcessMovement();
        ProcessRotation();
    }

    protected virtual void UpdateMapSpacePosition(IntVector3 position) {
        ITileInfo oldTileInfo = LevelDataManager.Instance.GetTileAt(_mapPosition.x, _mapPosition.y);
        oldTileInfo.RemoveOccupant(_unit);
        _mapPosition = position;
        ITileInfo newTileInfo = LevelDataManager.Instance.GetTileAt(_mapPosition.x, _mapPosition.y);
        newTileInfo.AddOccupant(_unit);
        OnMapPositionUpdated?.Invoke(MapPosition);
    }

    private void OnGamePause(bool paused) {
        _active = !paused;
    }
}
