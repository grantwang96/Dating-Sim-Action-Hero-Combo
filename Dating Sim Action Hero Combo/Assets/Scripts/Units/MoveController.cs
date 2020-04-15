using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class MoveController : MonoBehaviour {

    [SerializeField] protected Rigidbody2D _rigidbody;

    public IntVector3 MapPosition { get; protected set; }

    protected abstract void ProcessMovement();
    protected abstract void ProcessRotation();

    public event Action<IntVector3> OnMapPositionUpdated;

    public virtual void Initialize() {
        UpdateMapSpacePosition(LevelDataManager.Instance.WorldToArraySpace(transform.position));
    }

    protected virtual void FixedUpdate() {
        ProcessMovement();
        ProcessRotation();
    }

    protected virtual void UpdateMapSpacePosition(IntVector3 position) {
        MapPosition = position;
        OnMapPositionUpdated?.Invoke(MapPosition);
    }
}
