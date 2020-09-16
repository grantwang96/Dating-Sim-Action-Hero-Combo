using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The game state for a typical game level scenario
/// </summary>
public class GameplayGameState : GameState
{
    private const string OnGameEndedTransition = "game_ended";

    protected override IInitializableManager[] _initializableManagers => new IInitializableManager[] {
        // game managers
        GameManager.Instance,
        LevelDataManager.Instance,
        UnitsManager.Instance,
        EnemyManager.Instance,
        QuestManager.Instance,

        // ui managers
        NPCUIDisplayManager.Instance,
        QuestInfoDisplayManager.Instance
    };

    protected override void OnStateEnterComplete() {
        base.OnStateEnterComplete();
        GameManager.Instance.OnGameEnded += OnGameEnded;
    }

    // temp: remove this when game over screen is created
    private void OnGameEnded() {
        GameManager.Instance.OnGameEnded -= OnGameEnded;
        GameStateManager.Instance.HandleTransition(OnGameEndedTransition);
    }
}
