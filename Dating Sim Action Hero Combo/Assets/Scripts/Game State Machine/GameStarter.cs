using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStarter : MonoBehaviour
{
    [SerializeField] private GameState _gameState;

    private void Awake() {
        _gameState.OnGameStateEnter += OnGameStateEntered;
    }

    private void OnDestroy() {
        _gameState.OnGameStateEnter -= OnGameStateEntered;
    }

    private void OnGameStateEntered() {
        GameManager.Instance.StartGame();
    }
}
