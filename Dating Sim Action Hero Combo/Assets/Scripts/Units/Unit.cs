using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class Unit : MonoBehaviour, ITileOccupant, IDetectable, PooledObject
{
    private const int UniqueIdLength = 10;

    [SerializeField] protected UnitTags _unitTags;
    public UnitTags UnitTags => _unitTags;
    [SerializeField] protected DetectableTags _detectableTags;
    
    [SerializeField] protected MoveController _moveController;
    [SerializeField] protected UnitDamageable _damageable;
    [SerializeField] protected UnitAnimationController _animationController;

    public string UnitId { get; protected set; }
    public UnitData UnitData { get; protected set; }
    
    public MoveController MoveController => _moveController;
    public IDamageable Damageable => _damageable;
    public IAnimationController AnimationController => _animationController;

    public Transform Transform => transform;
    public DetectableTags DetectableTags => _detectableTags;

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

    public virtual void Initialize(PooledObjectInitializationData initializationData) {
        UnitInitializationData initData = initializationData as UnitInitializationData;
        if (initData == null) {
            Debug.LogError($"[{name}]: Initialization data was not of type {nameof(UnitInitializationData)}!");
            return;
        }
        if (!string.IsNullOrEmpty(initData.OverrideUniqueId)) {
            UnitId = initData.OverrideUniqueId;
        } else {
            UnitId = GenerateUniqueUnitId(initData.UnitData.UnitPrefabId);
        }
        UnitData = initData.UnitData;
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

    public abstract void Spawn();
    public abstract void Despawn();

    protected virtual void SubscribeToEvents() {
        _damageable.OnDefeated += OnDefeated;
    }

    protected virtual void UnsubscribeToEvents() {
        _damageable.OnDefeated -= OnDefeated;
    }

    protected static string GenerateUniqueUnitId(string prefix) {
        return $"{prefix}_{StringGenerator.RandomString(UniqueIdLength)}";
    }

    protected virtual void OnDefeated() {
        OnUnitDefeated?.Invoke(this);
    }
}

public class UnitInitializationData : PooledObjectInitializationData
{
    public string OverrideUniqueId;
    public UnitData UnitData;
}

[System.Flags]
public enum UnitTags {
    Player = (1 << 0),
    Civilian = (1 << 1),
    Law_Enforcement = (1 << 2),
    Enemy = (1 << 3),
    Date = (1 << 4),
    Agent = (1 << 5)
}
