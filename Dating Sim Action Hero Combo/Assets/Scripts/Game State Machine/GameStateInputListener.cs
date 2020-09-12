using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateInputListener : MonoBehaviour
{
    [SerializeField] private GameState _gameState;

    private void Awake() {
        _gameState.OnGameStateEnter += OnGameStateEnter;
        _gameState.OnGameStateExit += OnGameStateExit;
    }

    private void OnDestroy() {
        _gameState.OnGameStateEnter -= OnGameStateEnter;
        _gameState.OnGameStateExit -= OnGameStateExit;
    }

    private void OnGameStateEnter() {
        
    }

    private void OnGameStateExit() {

    }
}
