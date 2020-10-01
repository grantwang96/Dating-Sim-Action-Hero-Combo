using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class Unit : MonoBehaviour, ITileOccupant
{
    [SerializeField] private UnitTags _unitTags;
    public UnitTags UnitTags => _unitTags;
    
    [SerializeField] protected MoveController _moveController;
    [SerializeField] protected UnitDamageable _damageable;
    [SerializeField] protected UnitAnimationController _animationController;

    public string UnitId { get; protected set; }
    public UnitData UnitData { get; protected set; }
    
    public MoveController MoveController => _moveController;
    public IDamageable Damageable => _damageable;
    public IAnimationController AnimationController => _animationController;
    
    public event Action OnUnitInitialized;
    public event Action<Unit> OnUnitDefeated;

    protected virtual void Awake() {

    }

    protected virtual void Start() {
        SubscribeToEvents();
    }

    protected virtual void OnDestroy() {
        UnsubscribeToEvents();
    }

    public virtual void Initialize(string unitId, UnitData unitData) {
        UnitId = unitId;
        UnitData = unitData;
        _damageable.Initialize(); // to prevent race condition
        _moveController.Initialize();
        _animationController.Initialize();
        SubscribeToEvents();
        OnUnitInitialized?.Invoke();
    }

    public virtual void Dispose() {
        _damageable.Dispose();
        _moveController.Dispose();
        _animationController.Dispose();
        UnsubscribeToEvents();
    }

    protected virtual void SubscribeToEvents() {
        _damageable.OnDefeated += OnDefeated;
    }

    protected virtual void UnsubscribeToEvents() {
        _damageable.OnDefeated -= OnDefeated;
    }

    protected virtual void OnDefeated() {
        OnUnitDefeated?.Invoke(this);
    }
}

[System.Flags]
public enum UnitTags {
    Player = (1 << 0),
    Civilian = (1 << 1),
    Law_Enforcement = (1 << 2),
    Enemy = (1 << 3),
    Date = (1 << 4)
}
