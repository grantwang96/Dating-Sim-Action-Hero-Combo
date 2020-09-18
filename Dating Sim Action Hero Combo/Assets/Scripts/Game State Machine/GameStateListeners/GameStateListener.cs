using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameStateListener : MonoBehaviour
{
    [SerializeField] protected GameState _gameState;
    
    protected virtual void Awake() {
        _gameState = GetComponent<GameState>();
        _gameState.OnGameStateEnter += OnGameStateEntered;
        _gameState.OnGameStateExit += OnGameStateExited;
    }

    protected virtual void OnDestroy() {
        _gameState.OnGameStateEnter -= OnGameStateEntered;
        _gameState.OnGameStateExit -= OnGameStateExited;
    }

    protected virtual void OnGameStateEntered() {

    }

    protected virtual void OnGameStateExited() {

    }
}
