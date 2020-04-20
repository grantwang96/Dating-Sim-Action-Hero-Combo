using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IEnemyManager {

    IReadOnlyList<EnemyController> AllEnemies { get; }

    event Action<AIStateTransitionId, NPCUnitController> OnUnitBroadcastMessage;
    event Action<UnitController> OnEnemyDefeated;

    void SpawnEnemy(Vector2 position, string enemyType, string overrideId);
    void DespawnEnemy(EnemyController controller);
}

public class EnemyManager : MonoBehaviour, IEnemyManager
{
    public static IEnemyManager Instance { get; private set; }
    [SerializeField] private List<EnemyData> _allEnemyTypes = new List<EnemyData>();

    private Dictionary<string, EnemyData> _enemyDataConfig = new Dictionary<string, EnemyData>();
    private List<EnemyController> _enemyControllers = new List<EnemyController>();
    // dictionary of enemies by job <Job, List<EnemyController>;

    [SerializeField] private EnemyManagerState _currentState;
    [SerializeField] private List<EM_StateTransitionEntry> _EMStateTransitionEntries = new List<EM_StateTransitionEntry>();
    private Dictionary<EM_StateTransitionId, EnemyManagerState> _stateList = new Dictionary<EM_StateTransitionId, EnemyManagerState>();

    public IReadOnlyList<EnemyController> AllEnemies => _enemyControllers;

    public event Action<AIStateTransitionId, NPCUnitController> OnUnitBroadcastMessage;
    public event Action<UnitController> OnEnemyDefeated;

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

        // create enemy controller for unit here
        EnemyController enemyController = new EnemyController(data, unit, overrideId);
        enemyController.OnAIStateReadyToTransition += OnAIStateReadyToTransition;
        enemyController.OnUnitDefeated += OnUnitDefeated;
        enemyController.TransitionState(AIStateTransitionId.OnUnitInitialized);

        // add enemy controller to list and dictionary
        _enemyControllers.Add(enemyController);
    }

    public void DespawnEnemy(EnemyController controller) {
        // remove from all listings
        controller.Dispose();
        _enemyControllers.Remove(controller);
    }
    
    private void OnAIStateReadyToTransition(AIStateTransitionId id, NPCUnitController controller) {
        _currentState.OnControllerReadyToTransition(id, controller);
    }

    private void BroadcastMessageToUnits(AIStateTransitionId id, NPCUnitController controller) {
        OnUnitBroadcastMessage?.Invoke(id, controller);
    }

    private void ChangeState(EnemyManagerState newState) {
        if(_currentState != null) {
            _currentState.Exit();
            _currentState.OnBroadcastUnitStateChange -= BroadcastMessageToUnits;
        }
        _currentState = newState;
        _currentState.Enter();
        _currentState.OnBroadcastUnitStateChange += BroadcastMessageToUnits;
    }

    private void OnUnitDefeated(UnitController unit) {
        unit.OnUnitDefeated -= OnUnitDefeated;
        OnEnemyDefeated?.Invoke(unit);
    }
}

[System.Serializable]
public class EM_StateTransitionEntry {
    public EM_StateTransitionId TransitionId;
    public EnemyManagerState State;
}