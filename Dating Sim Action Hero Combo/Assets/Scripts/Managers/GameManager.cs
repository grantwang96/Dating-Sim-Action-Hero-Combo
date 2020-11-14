using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Manages what happens when the game starts and ends (determines when/how the game ends when played to completion)
/// </summary>
public class GameManager : IInitializableManager {

    private const string ContinueToResultsTransition = "continue_to_results";
    private const string GameExitTransition = "exit_game";
    private const string AllQuestsCompletedTimerId = "AllQuestsCompletedDisplay";
    private const float AllQuestsCompletedEndTime = 3f;

    public static GameManager Instance => GetOrSetInstance();
    private static GameManager _instance;

    public bool GamePaused { get; private set; }

    public EndGameContext EndGameContext { get; private set; }
    
    public void Initialize(Action<bool> initializationCallback = null) {

        GamePaused = false;

        initializationCallback?.Invoke(true);
    }

    private static GameManager GetOrSetInstance() {
        if(_instance == null) {
            _instance = new GameManager();
        }
        return _instance;
    }

    public void Dispose() {
        UnsubscribeToGameEndTriggers();
        GameEventsManager.ExitGame?.Broadcast();
    }

    // game begins, player is given control, the first quest appears, etc.
    public void StartGame() {
        Debug.Log($"[{nameof(GameManager)}]: Starting game...");
        GameEventsManager.StartGame?.Broadcast();
        SubscribeToGameStartTriggers();
    }

    public void EndGame() {
        UnsubscribeToGameEndTriggers();
        GameEventsManager.EndGame?.Broadcast(EndGameContext);
        TimerManager.Instance.AddTimer(new SimpleActionTimer(AllQuestsCompletedTimerId, AllQuestsCompletedEndTime, ContinueFromGameEnd));
    }

    public void ExitGame() {
        UnsubscribeToGameEndTriggers();
        GameEventsManager.ExitGame?.Broadcast();
        GameStateManager.Instance.HandleTransition(GameExitTransition);
    }

    public void TogglePauseGame() { 
        GamePaused = !GamePaused;
        GameEventsManager.PauseMenu?.Broadcast(GamePaused);
    }
    
    // how you win the game
    private void OnAllQuestsCompleted() {
        CustomLogger.Log(nameof(GameManager), "All quests completed! Ending game...");
        EndGameContext = new EndGameContext(true, EndResult.AllQuestsCompleted);
        EndGame();
    }

    // if the player is defeated, this should result in a loss
    private void OnPlayerDefeated(Unit unit) {
        CustomLogger.Log(nameof(GameManager), "Player defeated! Ending game...");
        EndGameContext = new EndGameContext(false, EndResult.PlayerDefeated);
        EndGame();
    }

    private void OnPlayerAgentSpotted() {
        CustomLogger.Log(nameof(GameManager), "Player spotted by date in agent gear! Ending game...");
        EndGameContext = new EndGameContext(false, EndResult.IdentityDiscovered);
        EndGame();
    }

    private void OnDateDefeated() {
        Debug.Log($"[{nameof(GameManager)}]: Date defeated! Ending game...");
        EndGameContext = new EndGameContext(false, EndResult.DateDefeated);
        EndGame();
    }

    private void SubscribeToGameStartTriggers() {
        QuestManager.Instance.OnAllQuestsCompleted += OnAllQuestsCompleted;
        PlayerUnit.Instance.OnUnitDefeated += OnPlayerDefeated;
        DateStateManager.Instance.OnPlayerAgentSpotted += OnPlayerAgentSpotted;
        DateStateManager.Instance.OnDateDefeated += OnDateDefeated;
    }

    private void UnsubscribeToGameEndTriggers() {
        QuestManager.Instance.OnAllQuestsCompleted -= OnAllQuestsCompleted;
        PlayerUnit.Instance.OnUnitDefeated -= OnPlayerDefeated;
        DateStateManager.Instance.OnPlayerAgentSpotted -= OnPlayerAgentSpotted;
        DateStateManager.Instance.OnDateDefeated -= OnDateDefeated;
    }
    
    // called to exit the gameplay state and enter the next state (end result screens, etc.)
    private void ContinueFromGameEnd() {
        GameStateManager.Instance.HandleTransition(ContinueToResultsTransition);
        GameEventsManager.EndGameResults.Broadcast();
    }
}

public class EndGameContext {

    public bool WonGame { get; }
    public EndResult EndResult { get; }

    public EndGameContext(bool wonGame, EndResult endResult) {
        WonGame = wonGame;
        EndResult = endResult;
    }
}

public enum EndResult {
    AllQuestsCompleted,
    PlayerDefeated,
    IdentityDiscovered,
    DateTurnedAway,
    DateDefeated
}

public static partial class GameEventsManager {
    public static GameEvent StartGame { get; } = new GameEvent();
    public static GameEvent<EndGameContext> EndGame { get; } = new GameEvent<EndGameContext>();
    public static GameEvent<bool> PauseMenu { get; } = new GameEvent<bool>();
    public static GameEvent EndGameResults { get; } = new GameEvent();
    public static GameEvent ExitGame { get; } = new GameEvent();
}
