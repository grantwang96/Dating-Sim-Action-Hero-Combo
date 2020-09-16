using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The game state for a typical game level scenario
/// </summary>
public class GameplayGameState : GameState
{
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
}
