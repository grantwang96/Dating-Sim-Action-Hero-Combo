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
        new GameManager(),
        new LevelDataManager(),
        new UnitsManager(),
        new EnemyManager(),
        new QuestManager(),

        // ui managers
        new NPCUIDisplayManager(),
        new QuestInfoDisplayManager()
    };
}
