using System;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyManager : IAllianceManager {

    event Action<NPCUnit> OnEnemySpawned;
    event Action<Unit> OnEnemyDefeated;

    void SpawnEnemy(EnemySpawnData spawnData);
    void DespawnEnemy(EnemyUnit controller);
}

public class EnemySpawnData
{
    public Vector2 Position;
    public string EnemyType;
    public string OverrideId;
    public string PatrolLoopId;
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
    private readonly Dictionary<string, IEnemyInfo> _enemyUnitsById = new Dictionary<string, IEnemyInfo>();

    public event Action<NPCUnit> OnEnemySpawned;
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
        _enemyUnitsById.Clear();
        LoadEnemyConfig();
        initializationCallback?.Invoke(true);
    }

    private void RemoveAllEnemies() {
        foreach(KeyValuePair<string, IEnemyInfo> keyPair in _enemyUnitsById) {
            RemoveEnemyDespawnTimer(keyPair.Value.Unit);
            keyPair.Value.Unit.Despawn();
        }
    }

    public void Dispose() {
        RemoveAllEnemies();
        _enemyUnitsById.Clear();
        _enemyDataConfig.Clear();
    }

    private void LoadEnemyConfig() {
        _enemyDataConfig.Clear();
        GameLevelData currentGameLevel = GameLevelDataController.Instance.CurrentGameLevelData;
        for(int i = 0; i < currentGameLevel.EnemyDatas.Count; i++) {
            _enemyDataConfig.Add(currentGameLevel.EnemyDatas[i].UnitPrefabId, currentGameLevel.EnemyDatas[i]);
        }
    }
    #endregion
    
    public void SpawnEnemy(EnemySpawnData spawnData) {
        EnemyData data;
        if (!_enemyDataConfig.TryGetValue(spawnData.EnemyType, out data)) {
            CustomLogger.Error(nameof(EnemyManager), $"Could not retrieve {nameof(EnemyData)} from id {spawnData.EnemyType}");
            return;
        }

        // Get a pooled enemy object
        PooledObject pooledObject;
        if (!PooledObjectManager.Instance.UsePooledObject(data.UnitPrefabId, out pooledObject)) {
            PooledObjectManager.Instance.RegisterPooledObject(data.UnitPrefabId, 1);
            CustomLogger.Log(nameof(EnemyManager), $"{data.UnitPrefabId} not yet registered with object pool. Registering now...");
            SpawnEnemy(spawnData);
            return;
        }
        EnemyUnit unit = pooledObject as EnemyUnit;
        if(unit == null) {
            CustomLogger.Error(nameof(EnemyManager), $"Pooled Object was not of type {nameof(EnemyUnit)}");
            return;
        }
        // prep unit
        unit.transform.position = spawnData.Position;
        UnitInitializationData initData = new UnitInitializationData() {
            OverrideUniqueId = spawnData.OverrideId,
            UnitData = data
        };
        unit.Initialize(initData);
        unit.CombatController.SetWeapon(data.EquippedWeapon);
        AddUnitListeners(unit);
        unit.Spawn();
        UnitsManager.Instance.RegisterUnit(unit);

        // add enemy controller to list and dictionary
        _enemyUnitsById.Add(unit.UnitId, new EnemyInfo(unit.UnitId, spawnData.PatrolLoopId, unit));
        OnEnemySpawned?.Invoke(unit);
    }

    public void DespawnEnemy(EnemyUnit unit) {
        // remove from all listings
        _enemyUnitsById.Remove(unit.UnitId);
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

    private void AddEnemyDespawnTimer(Unit enemy) {
        string id = string.Format(DespawnUnitId, enemy.UnitId);
        TimerManager.Instance.AddTimer(new DespawnEnemyTimer(enemy, id, DespawnTime));
    }

    private void RemoveEnemyDespawnTimer(Unit enemy) {
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

    private Unit _unit;

    public DespawnEnemyTimer(Unit unit, string id, float duration) : base(id, duration) {
        _unit = unit;
    }

    protected override void DoTimerAction() {
        _unit.Despawn();
        base.DoTimerAction();
    }
}