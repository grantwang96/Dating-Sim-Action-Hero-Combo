using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Class that handles processing the current game scenario
/// </summary>
public class GameState : MonoBehaviour {

    [SerializeField] private List<GameStateTransition> _transitions = new List<GameStateTransition>(); // list of transitions this can go to
    public IReadOnlyList<GameStateTransition> Transitions => _transitions;
    [SerializeField] private string _sceneName;

    // lists of asset names required for this state
    [SerializeField] private List<PooledObjectLoadEntry> _pooledObjectPrefabAssets = new List<PooledObjectLoadEntry>();
    [SerializeField] private List<string> _uiPrefabAssets = new List<string>();

    private bool _initialized = false;

    public bool IsLoading { get; protected set; }
    public bool IsActive { get; protected set; }
    public GameState ParentState { get; protected set; }
    private List<GameState> _childStates = new List<GameState>();
    
    public event Action OnGameStateEnter;
    public event Action OnGameStateEnterFailed;
    public event Action OnGameStateExit;

    public bool Active;

    private void Awake() {
        if (_initialized) { return; }
        LoadParentState();
        LoadChildStates();
        _initialized = true;
    }
    
    // attempt to enter this state
    public virtual void Enter() {
        IsLoading = true;
        // enter parent state first, if necessary
        if (ParentState != null && !ParentState.IsActive) {
            ParentState.OnGameStateEnter += OnReadyToEnter;
            ParentState.Enter();
            return;
        }
        OnReadyToEnter();
    }

    // get a game state using a given transition id
    public GameState GetGameStateByTransitionName(string transitionName) {
        for (int i = 0; i < _transitions.Count; i++) {
            if (transitionName.Equals(_transitions[i].TransitionName)) {
                return _transitions[i].GameState;
            }
        }
        if (ParentState != null) {
            return ParentState.GetGameStateByTransitionName(transitionName);
        }
        return null;
    }

    // attempt to exit this state
    public virtual void Exit(GameState nextState) {
        // check to see if we actually need to exit
        if (nextState.StateOnPath(this)) {
            return;
        }
        IsLoading = false;
        IsActive = false;
        Active = true;
        DeregisterPrefabs();
        DisposeManagers();
        OnGameStateExit?.Invoke();
    }

    // check if a given state is on this state's active path
    public bool StateOnPath(GameState state) {
        if (ParentState == null) {
            return false;
        }
        if (this == state) {
            return true;
        }
        return ParentState.StateOnPath(state);
    }

    // initialize this state's parent state
    private void LoadParentState() {
        if(transform.parent == null) {
            return;
        }
        GameState parentState = transform.parent.GetComponent<GameState>();
        if(parentState != null) {
            ParentState = parentState;
        }
    }

    // initialize this state's children state
    private void LoadChildStates() {
        GameState[] childStates = GetComponentsInChildren<GameState>();
        if (childStates != null && childStates.Length != 0) {
            _childStates = new List<GameState>(childStates);
        }
    }

    protected void OnReadyToEnter() {
        if (RequiresScene()) {
            // that means a scene transition is necessary and we haven't fully loaded yet
            TryToLoadScene();
            return;
        }
        OnStateEnterSuccess();
    }

    private void TryToLoadScene() {
        bool success = SceneController.Instance.TransitionToScene(_sceneName);
        if (!success) {
            CustomLogger.Error($"[{this.name}]", $"[{nameof(GameState)}] Failed to find and load scene!");
            OnStateEnterFailed();
            return;
        }
        SceneController.Instance.OnSceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(string sceneName) {
        if (sceneName.Equals(_sceneName)) {
            SceneController.Instance.OnSceneLoaded -= OnSceneLoaded;
            OnStateEnterSuccess();
        }
    }

    // when the game the state is ready to initialize (scene has been loaded, parents are ready, etc.)
    protected virtual void OnStateEnterSuccess() {
        IsLoading = false;
        IsActive = true;
        Active = true;
        // the first step of the chain is initializing the managers
        InitializeManagers();
    }

    protected virtual void InitializeManagers() {
        OnManagersInitializationComplete();
    }

    protected void OnManagersInitializationComplete() {
        RegisterPrefabs();
        if (ParentState != null) {
            ParentState.OnGameStateEnter -= OnStateEnterSuccess;
        }
        OnGameStateEnter?.Invoke();
    }

    protected virtual void DisposeManagers() {
        
    }

    private void RegisterPrefabs() {
        for (int i = 0; i < _pooledObjectPrefabAssets.Count; i++) {
            PooledObjectManager.Instance.RegisterPooledObject(_pooledObjectPrefabAssets[i].PoolId, _pooledObjectPrefabAssets[i].InitialCount);
        }
    }

    private void DeregisterPrefabs() {
        for (int i = 0; i < _pooledObjectPrefabAssets.Count; i++) {
            PooledObjectManager.Instance.DeregisterPooledObject(_pooledObjectPrefabAssets[i].PoolId);
        }
    }

    protected bool RequiresScene() {
        return !string.IsNullOrEmpty(_sceneName);
    }

    private void OnStateEnterFailed() {
        IsActive = false;
        Active = false;
        IsLoading = false;
        OnGameStateEnterFailed?.Invoke();
        CustomLogger.Error(this.name, $"Failed to enter state!");
    }
}

[System.Serializable]
public class GameStateTransition {
    [SerializeField] private string _transitionName;
    [SerializeField] private GameState _gameState;

    public string TransitionName => _transitionName;
    public GameState GameState => _gameState;
}
