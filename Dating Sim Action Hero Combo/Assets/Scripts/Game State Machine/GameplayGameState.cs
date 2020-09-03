using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The game state for a typical game level scenario
/// </summary>
public class GameplayGameState : GameState
{
    private int _currentManagerIndex;

    private IInitializableManager[] _initializableManagers = {
        // game managers
        new GameManager(),
        new LevelDataManager(),
        new UnitsManager(),
        new EnemyManager(),
        new QuestManager(),

        // ui managers
        new NPCUIDisplayManager()
    };

    protected override void InitializeManagers() {
        _currentManagerIndex = 0;
        _initializableManagers[0].Initialize(ManagerInitializationCallback);
    }

    protected override void OnStateEnterSuccess() {
        base.OnStateEnterSuccess();
        GameManager.Instance.StartGame();
    }

    protected void ManagerInitializationCallback(bool success) {
        if (!success) {
            CustomLogger.Error(name, $"Failed to initialize manager at index {_currentManagerIndex}!");
        }
        _currentManagerIndex++;
        if(_currentManagerIndex >= _initializableManagers.Length) {
            OnManagersInitializationComplete();
            return;
        }
        _initializableManagers[_currentManagerIndex].Initialize(ManagerInitializationCallback);
    }

    protected override void DisposeManagers() {
        base.DisposeManagers();
        for(int i = 0; i < _initializableManagers.Length; i++) {
            _initializableManagers[i].Dispose();
        }
    }
}
