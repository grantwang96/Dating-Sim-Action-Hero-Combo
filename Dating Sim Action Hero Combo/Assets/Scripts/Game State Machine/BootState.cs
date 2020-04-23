using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootState : GameState
{
    [SerializeField] private PlayerConfig _playerConfig;

    protected override void OnStateEnterSuccess() {
        base.OnStateEnterSuccess();
        InitializeManagers();
        GameStateManager.Instance.HandleTransition(Transitions[0].TransitionName);
    }

    private void InitializeManagers() {
        PlayerStateController.Create(_playerConfig);
    }
}
