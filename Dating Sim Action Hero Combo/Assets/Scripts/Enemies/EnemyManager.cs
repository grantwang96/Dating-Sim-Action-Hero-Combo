using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyManager {
    void SpawnEnemy(Vector2 position, string enemyType);
    void ChangeState(EnemyManagerState newState);
}

public class EnemyManager : MonoBehaviour, IEnemyManager
{
    public static IEnemyManager Instance { get; private set; }
    [SerializeField] private List<EnemyData> _allEnemyTypes = new List<EnemyData>();

    private EnemyManagerState _currentState;
    private Dictionary<string, EnemyData> _enemyDataConfig = new Dictionary<string, EnemyData>();
    private List<IEnemyController> _enemyControllers = new List<IEnemyController>();
    // dictionary of enemies by job <Job, List<EnemyController>

    private void Awake() {
        Instance = this;
        LoadEnemyConfig();
        ChangeState(new DefaultEnemyManagerState());
    }

    private void LoadEnemyConfig() {
        for(int i = 0; i < _allEnemyTypes.Count; i++) {
            _enemyDataConfig.Add(_allEnemyTypes[i].name, _allEnemyTypes[i]);
        }
    }

    private void Update() {
        ProcessEnemyControllers();
    }

    private void ProcessEnemyControllers() {
        for(int i = 0; i < _enemyControllers.Count; i++) {
            _enemyControllers[i].ExecuteState();
        }
    }

    public void SpawnEnemy(Vector2 position, string enemyType) {
        EnemyData data;
        if (!_enemyDataConfig.TryGetValue(enemyType, out data)) {
            CustomLogger.Error(nameof(EnemyManager), $"Could not retrieve {nameof(EnemyData)} from id {enemyType}");
            return;
        }

        // Get a pooled enemy object
        PooledObject pooledObject;
        if (!PooledObjectManager.Instance.UsePooledObject(data.UnitPrefabId, out pooledObject)) {
            CustomLogger.Log(nameof(EnemyManager), $"{data.UnitPrefabId} not yet registered with object pool. Registering now...");
            PooledObjectManager.Instance.RegisterPooledObject(data.UnitPrefabId, 1);
            if(!PooledObjectManager.Instance.UsePooledObject(data.UnitPrefabId, out pooledObject)) {
                CustomLogger.Error(nameof(EnemyManager), $"Could not retrieve pooled object resource with id {data.UnitPrefabId}");
                return;
            }
        }
        EnemyUnit unit = pooledObject as EnemyUnit;
        if(unit == null) {
            CustomLogger.Error(nameof(EnemyManager), $"Pooled Object was not of type {nameof(EnemyUnit)}");
            return;
        }
        // prep unit
        unit.transform.position = position;

        // create enemy controller for unit here
        IEnemyController enemyController = new EnemyController(data, unit);
        enemyController.OnAIStateReadyToTransition += OnAIStateReadyToTransition;
        enemyController.TransitionState(AIStateTransitionId.OnUnitInitialized);

        // add enemy controller to list and dictionary
        _enemyControllers.Add(enemyController);

        // spawn the unit
        unit.Spawn();
    }

    private void OnAIStateReadyToTransition(AIStateTransitionId id, IEnemyController controller) {
        _currentState.OnReadyToTransition(id, controller);
    }

    public void ChangeState(EnemyManagerState newState) {
        if(_currentState != null) {
            _currentState.Exit();
        }
        _currentState = newState;
        _currentState.Enter();
    }
}
