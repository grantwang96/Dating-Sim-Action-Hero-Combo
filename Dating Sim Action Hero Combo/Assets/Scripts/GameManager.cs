using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : IInitializableManager
{
    public static GameManager Instance => GetOrSetInstance();
    private static GameManager _instance;

    public event Action OnGameStarted;
    public event Action OnGameEnded;

    public void Initialize(Action<bool> initializationCallback = null) {
        initializationCallback?.Invoke(true);
    }

    private static GameManager GetOrSetInstance() {
        if(_instance == null) {
            _instance = new GameManager();
        }
        return _instance;
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
        // todo: enter the game over state
    }
}

public static partial class GameEventsManager {
    public static GameEvent StartGame { get; } = new GameEvent();
    public static GameEvent<bool> EndGame { get; } = new GameEvent<bool>();
}
