using System;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyManager : IAllianceManager {

    IReadOnlyList<EnemyUnit> AllEnemies { get; }
    
    event Action<Unit> OnEnemySpawned;
    event Action<Unit> OnEnemyDefeated;

    void SpawnEnemy(Vector2 position, string enemyType, string overrideId);
    void DespawnEnemy(EnemyUnit controller);
}

/// <summary>
/// Manages spawning and messaging of hostile units
/// </summary>
public class EnemyManager : IEnemyManager
{
    private const float DespawnTime = 3f;
    private const string DespawnUnitId = "despawn_unit_{0}";

    public static IEnemyManager Instance => GetOrSetInstance();
    private static IEnemyManager _instance;

    private Dictionary<string, EnemyData> _enemyDataConfig = new Dictionary<string, EnemyData>();
    private List<EnemyUnit> _enemyUnits = new List<EnemyUnit>();
    // dictionary of enemies by job <Job, List<EnemyController>;

    public IReadOnlyList<EnemyUnit> AllEnemies => _enemyUnits;
    public event Action<Unit> OnEnemySpawned;
    public event Action<Unit> OnEnemyDefeated;
    public event Action<NPCUnit, UnitMessage> OnAllianceMessageSent;

    private static IEnemyManager GetOrSetInstance() {
        if(_instance == null) {
            _instance = new EnemyManager();
        }
        return _instance;
    }

    #region INITIALIZATION
    public void Initialize(Action<bool> initializationCallback = null) {
        _enemyUnits.Clear();
        LoadEnemyConfig();
        initializationCallback?.Invoke(true);
    }

    private void RemoveAllEnemies() {
        for (int i = 0; i < _enemyUnits.Count; i++) {
            RemoveEnemyDespawnTimer(_enemyUnits[i]);
            _enemyUnits[i].Despawn();
        }
    }

    public void Dispose() {
        RemoveAllEnemies();
        _enemyUnits.Clear();
        _enemyDataConfig.Clear();
    }

    private void LoadEnemyConfig() {
        _enemyDataConfig.Clear();
        GameLevelData currentGameLevel = GameLevelDataController.Instance.CurrentGameLevelData;
        for(int i = 0; i < currentGameLevel.EnemyDatas.Count; i++) {
            _enemyDataConfig.Add(currentGameLevel.EnemyDatas[i].name, currentGameLevel.EnemyDatas[i]);
        }
    }
    #endregion
    
    public void SpawnEnemy(Vector2 position, string enemyType, string overrideId) {
        EnemyData data;
        if (!_enemyDataConfig.TryGetValue(enemyType, out data)) {
            CustomLogger.Error(nameof(EnemyManager), $"Could not retrieve {nameof(EnemyData)} from id {enemyType}");
            return;
        }

        // Get a pooled enemy object
        PooledObject pooledObject;
        if (!PooledObjectManager.Instance.UsePooledObject(data.UnitPrefabId, out pooledObject)) {
            PooledObjectManager.Instance.RegisterPooledObject(data.UnitPrefabId, 1);
            CustomLogger.Log(nameof(EnemyManager), $"{data.UnitPrefabId} not yet registered with object pool. Registering now...");
            SpawnEnemy(position, enemyType, overrideId);
            return;
        }
        EnemyUnit unit = pooledObject as EnemyUnit;
        if(unit == null) {
            CustomLogger.Error(nameof(EnemyManager), $"Pooled Object was not of type {nameof(EnemyUnit)}");
            return;
        }
        // prep unit
        unit.transform.position = position;
        unit.Initialize(overrideId, data);
        unit.CombatController.SetWeapon(data.EquippedWeapon);
        AddUnitListeners(unit);
        unit.Spawn();
        UnitsManager.Instance.RegisterUnit(unit);

        // add enemy controller to list and dictionary
        _enemyUnits.Add(unit);
        OnEnemySpawned?.Invoke(unit);
    }

    public void DespawnEnemy(EnemyUnit unit) {
        // remove from all listings
        _enemyUnits.Remove(unit);
        unit.Despawn();
        string timerId = string.Format(DespawnUnitId, unit.UnitId);
        if(TimerManager.Instance.TryGetTimer(timerId, out TimerObject _)) {
            TimerManager.Instance.RemoveTimer(timerId);
        }
        UnitsManager.Instance.DeregisterUnit(unit);
    }

    // listen for unit events like being defeated or sending messages
    private void AddUnitListeners(EnemyUnit unit) {
        unit.OnUnitDefeated += OnUnitDefeated;
        unit.OnUnitMessageSent += OnUnitMessageReceived;
    }

    // remove listeners once unit has been defeated
    private void RemoveUnitListeners(EnemyUnit unit) {
        unit.OnUnitDefeated -= OnUnitDefeated;
        unit.OnUnitMessageSent -= OnUnitMessageReceived;
    }

    private void OnUnitDefeated(Unit unit) {
        EnemyUnit enemy = unit as EnemyUnit;
        if(enemy == null) {
            return;
        }
        RemoveUnitListeners(enemy);
        AddEnemyDespawnTimer(enemy);
         OnEnemyDefeated?.Invoke(unit);
    }

    private void AddEnemyDespawnTimer(EnemyUnit enemy) {
        string id = string.Format(DespawnUnitId, enemy.UnitId);
        TimerManager.Instance.AddTimer(new DespawnEnemyTimer(enemy, id, DespawnTime));
    }

    private void RemoveEnemyDespawnTimer(EnemyUnit enemy) {
        string timerId = string.Format(DespawnUnitId, enemy.UnitId);
        TimerManager.Instance.RemoveTimer(timerId);
    }

    // process certain unit messages in order to message other units in the "alliance"
    private void OnUnitMessageReceived(NPCUnit ally, UnitMessage message) {
        switch (message) {
            case UnitMessage.HostileFound:
            case UnitMessage.PlayerObjectiveInProgress:
            case UnitMessage.PlayerObjectiveCompleted:
                OnAllianceMessageSent?.Invoke(ally, message);
                break;
            default:
                break;
        }
    }
}

public class DespawnEnemyTimer : TimerObject {

    private EnemyUnit _unit;

    public DespawnEnemyTimer(EnemyUnit unit, string id, float duration) : base(id, duration) {
        _unit = unit;
    }

    protected override void DoTimerAction() {
        _unit.Despawn();
        base.DoTimerAction();
    }
}