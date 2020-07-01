using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class MoveController : MonoBehaviour {

    [SerializeField] protected Rigidbody2D _rigidbody;
    [SerializeField] protected Transform _body;
    [SerializeField] protected Transform _front;

    public IntVector3 MapPosition { get; protected set; }
    public Transform Body => _body;
    public Transform Front => _front;

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
