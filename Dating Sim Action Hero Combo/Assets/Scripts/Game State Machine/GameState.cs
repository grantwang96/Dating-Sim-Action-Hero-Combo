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
    [SerializeField] private bool _requiresLoadingScreen;

    // lists of asset names required for this state
    [SerializeField] private List<PooledObjectLoadEntry> _pooledObjectPrefabAssets = new List<PooledObjectLoadEntry>();
    [SerializeField] private List<UIPrefabEntry> _uiPrefabs = new List<UIPrefabEntry>();

    private bool _initialized = false;
    protected int _currentManagerIndex;
    protected virtual IInitializableManager[] _initializableManagers { get; }

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
        Debug.Log($"Entering game state {name}");
        IsLoading = true;
        // enter parent state first, if necessary
        if (ParentState != null && !ParentState.IsActive) {
            ParentState.OnGameStateEnter += OnParentStateEntered;
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
            Debug.Log($"No need to exit state {name}");
            return;
        }
        // have the parent exit as well if necessary
        if (ParentState != null) {
            ParentState.Exit(nextState);
        }
        ConfirmExitState();
    }

    protected virtual void ConfirmExitState() {
        Debug.Log($"Exiting state {name}...");
        IsLoading = false;
        IsActive = false;
        Active = false;
        DisposeManagers();
        DeregisterPooledObjectPrefabs();
        DeregisterUIPrefabs();
        OnGameStateExit?.Invoke();
    }

    // check if a given state is on this state's active path
    public bool StateOnPath(GameState state) {
        if (this == state) {
            return true;
        }
        if (ParentState == null) {
            return false;
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

    protected void OnParentStateEntered() {
        ParentState.OnGameStateEnter -= OnParentStateEntered;
        OnReadyToEnter();
    }

    protected void OnReadyToEnter() {
        CustomLogger.Log(name, $"Ready to enter {name}");
        if (RequiresScene()) {
            // that means a scene transition is necessary and we haven't fully loaded yet
            TryToLoadScene();
            return;
        }
        OnStateEnterSuccess();
    }

    private void TryToLoadScene() {
        bool success = SceneController.Instance.TransitionToScene(_sceneName, _requiresLoadingScreen);
        if (!success) {
            CustomLogger.Error(name, $"[{nameof(GameState)}] Failed to find and load scene!");
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
        CustomLogger.Log(name, $"Successfully entered state {name}!");
        IsLoading = false;
        IsActive = true;
        Active = true;
        // the first step of the chain is initializing the managers
        InitializeManagers();
    }

    protected virtual void InitializeManagers() {
        if (_initializableManagers == null) {
            OnManagersInitializationComplete();
            return;
        }
        _currentManagerIndex = 0;
        _initializableManagers[_currentManagerIndex].Initialize(ManagerInitializationCallback);
    }

    protected void ManagerInitializationCallback(bool success) {
        if (!success) {
            CustomLogger.Error(name, $"Failed to initialize manager at index {_currentManagerIndex}!");
        }
        _currentManagerIndex++;
        if (_currentManagerIndex >= _initializableManagers.Length) {
            OnManagersInitializationComplete();
            return;
        }
        _initializableManagers[_currentManagerIndex].Initialize(ManagerInitializationCallback);
    }

    protected void OnManagersInitializationComplete() {
        RegisterPooledObjectPrefabs();
        RegisterUIPrefabs();
        OnStateEnterComplete();
    }

    protected virtual void OnStateEnterComplete() {
        if (ParentState != null) {
            ParentState.OnGameStateEnter -= OnStateEnterSuccess;
        }
        OnGameStateEnter?.Invoke();
    }

    protected void DisposeManagers() {
        if(_initializableManagers == null) {
            return;
        }
        for (int i = 0; i < _initializableManagers.Length; i++) {
            _initializableManagers[i].Dispose();
        }
    }

    private void RegisterPooledObjectPrefabs() {
        for (int i = 0; i < _pooledObjectPrefabAssets.Count; i++) {
            PooledObjectManager.Instance.RegisterPooledObject(_pooledObjectPrefabAssets[i].PoolId, _pooledObjectPrefabAssets[i].InitialCount);
        }
    }

    private void RegisterUIPrefabs() {
        for(int i = 0; i < _uiPrefabs.Count; i++) {
            UIObject uiObject = UIManager.Instance.CreateNewUIObject(_uiPrefabs[i].UIPrefabId, _uiPrefabs[i].LayerID);
            if (uiObject == null) {
                CustomLogger.Error(this.name, $"Could not retrieve ui object with id {_uiPrefabs[i].UIPrefabId}");
                continue;
            }
            uiObject.Initialize();
            uiObject.Display();
        }
    }

    private void DeregisterPooledObjectPrefabs() {
        for (int i = 0; i < _pooledObjectPrefabAssets.Count; i++) {
            string poolId = _pooledObjectPrefabAssets[i].PoolId;
            Debug.Log($"Deregistering {poolId}...");
            PooledObjectManager.Instance.DeregisterPooledObject(poolId);
        }
    }

    private void DeregisterUIPrefabs() {
        for (int i = 0; i < _uiPrefabs.Count; i++) {
            UIObject uiObject = UIManager.Instance.GetUIObject(_uiPrefabs[i].UIPrefabId);
            if(uiObject == null) {
                CustomLogger.Error(this.name, $"Could not retrieve ui objet with id {_uiPrefabs[i].UIPrefabId}");
                continue;
            }
            uiObject.Hide();
            UIManager.Instance.RemoveUIObject(_uiPrefabs[i].UIPrefabId);
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
