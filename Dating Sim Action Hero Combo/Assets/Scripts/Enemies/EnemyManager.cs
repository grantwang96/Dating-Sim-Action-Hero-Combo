using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyManager {

    IReadOnlyList<IEnemyController> AllEnemies { get; }

    void SpawnEnemy(Vector2 position, string enemyType);
    void DespawnEnemy(IEnemyController controller);
}

public class EnemyManager : MonoBehaviour, IEnemyManager
{
    public static IEnemyManager Instance { get; private set; }
    [SerializeField] private List<EnemyData> _allEnemyTypes = new List<EnemyData>();

    private Dictionary<string, EnemyData> _enemyDataConfig = new Dictionary<string, EnemyData>();
    private List<IEnemyController> _enemyControllers = new List<IEnemyController>();
    // dictionary of enemies by job <Job, List<EnemyController>;

    [SerializeField] private EnemyManagerState _currentState;
    [SerializeField] private List<EM_StateTransitionEntry> _EMStateTransitionEntries = new List<EM_StateTransitionEntry>();
    private Dictionary<EM_StateTransitionId, EnemyManagerState> _stateList = new Dictionary<EM_StateTransitionId, EnemyManagerState>();

    public IReadOnlyList<IEnemyController> AllEnemies => _enemyControllers;

    #region INITIALIZATION
    private void Awake() {
        Instance = this;
        LoadEnemyConfig();
        LoadManagerStates();
        ChangeState(_currentState);
    }

    private void LoadEnemyConfig() {
        for(int i = 0; i < _allEnemyTypes.Count; i++) {
            _enemyDataConfig.Add(_allEnemyTypes[i].name, _allEnemyTypes[i]);
        }
    }

    private void LoadManagerStates() {
        _stateList.Clear();
        for (int i = 0; i < _EMStateTransitionEntries.Count; i++) {
            _stateList.Add(_EMStateTransitionEntries[i].TransitionId, _EMStateTransitionEntries[i].State);
        }
    }
    #endregion

    #region UPDATE LOOP
    private void Update() {
        ProcessEnemyControllers();
    }

    private void ProcessEnemyControllers() {
        for(int i = 0; i < _enemyControllers.Count; i++) {
            _enemyControllers[i].ExecuteState();
        }
    }
    #endregion

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
    }

    public void DespawnEnemy(IEnemyController controller) {
        // remove from all listings
        controller.Dispose();
        _enemyControllers.Remove(controller);
        CustomLogger.Log(nameof(EnemyManager), $"Removing enemy controller from list. Count is now {_enemyControllers.Count}.");
    }
    
    private void OnAIStateReadyToTransition(AIStateTransitionId id, IEnemyController controller) {
        _currentState.OnControllerReadyToTransition(id, controller);
    }

    private void ChangeState(EnemyManagerState newState) {
        if(_currentState != null) {
            _currentState.Exit();
        }
        _currentState = newState;
        _currentState.Enter();
    }
}

[System.Serializable]
public class EM_StateTransitionEntry {
    public EM_StateTransitionId TransitionId;
    public EnemyManagerState State;
}