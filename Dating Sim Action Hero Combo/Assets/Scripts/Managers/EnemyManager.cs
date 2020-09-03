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

public class EnemyManager : IEnemyManager
{
    public static IEnemyManager Instance { get; private set; }

    private Dictionary<string, EnemyData> _enemyDataConfig = new Dictionary<string, EnemyData>();
    private List<EnemyUnit> _enemyUnits = new List<EnemyUnit>();
    // dictionary of enemies by job <Job, List<EnemyController>;

    public IReadOnlyList<EnemyUnit> AllEnemies => _enemyUnits;
    public event Action<Unit> OnEnemySpawned;
    public event Action<Unit> OnEnemyDefeated;
    public event Action<NPCUnit, UnitMessage> OnAllianceMessageSent;

    #region INITIALIZATION
    public void Initialize(Action<bool> initializationCallback = null) {
        Instance = this;
        LoadEnemyConfig();
        initializationCallback?.Invoke(true);
    }

    public void Dispose() {

    }

    private void LoadEnemyConfig() {
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

        // add enemy controller to list and dictionary
        _enemyUnits.Add(unit);
        OnEnemySpawned?.Invoke(unit);
    }

    public void DespawnEnemy(EnemyUnit unit) {
        // remove from all listings
        _enemyUnits.Remove(unit);
        unit.Despawn();
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
        OnEnemyDefeated?.Invoke(unit);
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