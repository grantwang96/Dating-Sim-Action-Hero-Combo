using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GameStateManager : MonoBehaviour {

    public static GameStateManager Instance { get; private set; }
    public GameState CurrentState { get; private set; }
    public GameState PreviousState { get; private set; }

    private bool _initialized = false;

    [SerializeField] private GameState _hackStartState; // where will the game start on load
    [SerializeField] private GameState _gameHubState; // where the game will normally boot to (if player has completed tutorial)
    [SerializeField] private GameState _tutorialState; // where the game will boot for first time users

    public event Action OnStateEntered;
    public event Action OnStateExited;

    private void Awake() {
        if (_initialized) { return; }
        if(Instance != null && Instance != this) {
            Debug.LogError($"[{nameof(GameStateManager)}] Game State Manager already exists!");
            return;
        }
        Instance = this;
    }

    // Use this for initialization
    void Start () {
        if (_initialized) { return; }
        Debug.Log($"[{nameof(GameStateManager)}] {name} is loaded!");
        OnAppStart();
	}

    // this is when the game initially loads. Should be on loading screen here
    private void OnAppStart() {
        ChangeState(_hackStartState);
        // InitializeGameState();
        _initialized = true;
    }

    private void InitializeGameState() {
        GameState startingState = GetStartingState();
        ChangeState(startingState);
    }

    private GameState GetStartingState() {
        /*
        if (!PlayerDataManager.Instance.GetFlag(GameplayValues.Tutorial.TutorialLevelCompletedId)) {
            return _tutorialState;
        }
        */
        return _gameHubState;
    }

    public void HandleTransition(string transitionName) {
        if(CurrentState == null) {
            Debug.LogError($"[{nameof(GameStateManager)}] Current State is null! Wait what?");
            return;
        }
        GameState nextState = CurrentState.GetGameStateByTransitionName(transitionName);
        if(nextState == null) {
            Debug.Log($"[{nameof(GameStateManager)}] Could not retrieve game state for transition name {transitionName}");
            return;
        }
        ChangeState(nextState);
    }

    private void ChangeState(GameState newState) {
        if(CurrentState != null) {
            PreviousState = CurrentState;
            CurrentState.Exit(newState);
            OnStateExited?.Invoke();
        }
        CurrentState = newState;
        CurrentState.Enter();
        OnStateEntered?.Invoke();
    }
}
