using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootState : GameState
{
    [SerializeField] private PlayerConfig _playerConfig;

    protected override void OnStateEnterSuccess() {
        base.OnStateEnterSuccess();
    }

    protected override void OnStateEnterComplete() {
        base.OnStateEnterComplete();
        GameStateManager.Instance.HandleTransition(Transitions[0].TransitionName);
    }

    protected override void InitializeManagers() {
        Debug.Log("Initializing boot state...");
        PlayerStateController.Create(_playerConfig);
        OnManagersInitializationComplete();
    }
}
