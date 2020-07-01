using System;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyManager {

    IReadOnlyList<EnemyUnit> AllEnemies { get; }

    event Action<Unit> OnEnemyDefeated;

    void SpawnEnemy(Vector2 position, string enemyType, string overrideId);
    void DespawnEnemy(EnemyUnit controller);
}

public class EnemyManager : MonoBehaviour, IEnemyManager
{
    public static IEnemyManager Instance { get; private set; }
    [SerializeField] private List<EnemyData> _allEnemyTypes = new List<EnemyData>();

    private Dictionary<string, EnemyData> _enemyDataConfig = new Dictionary<string, EnemyData>();
    private List<EnemyUnit> _enemyUnits = new List<EnemyUnit>();
    // dictionary of enemies by job <Job, List<EnemyController>;

    public IReadOnlyList<EnemyUnit> AllEnemies => _enemyUnits;
    public event Action<Unit> OnEnemyDefeated;

    #region INITIALIZATION
    private void Awake() {
        Instance = this;
        LoadEnemyConfig();
    }

    private void LoadEnemyConfig() {
        for(int i = 0; i < _allEnemyTypes.Count; i++) {
            _enemyDataConfig.Add(_allEnemyTypes[i].name, _allEnemyTypes[i]);
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

        // add enemy controller to list and dictionary
        _enemyUnits.Add(unit);
    }

    public void DespawnEnemy(EnemyUnit controller) {
        // remove from all listings
        _enemyUnits.Remove(controller);
    }

    private void OnUnitDefeated(Unit unit) {
        OnEnemyDefeated?.Invoke(unit);
    }
}