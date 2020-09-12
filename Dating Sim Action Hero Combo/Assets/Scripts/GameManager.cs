using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : IInitializableManager
{
    public static GameManager Instance { get; private set; }

    public event Action OnGameStarted;
    public event Action OnGameEnded;

    public void Initialize(Action<bool> initializationCallback = null) {
        Instance = this;
        initializationCallback?.Invoke(true);
    }

    public void Dispose() {

    }

    // game begins, player is given control, the first quest appears, etc.
    public void StartGame() {
        OnGameStarted?.Invoke();
    }

    // called by completing all quests, losing the game, exiting the game
    public void EndGame() {
        OnGameEnded?.Invoke();
    }
}
