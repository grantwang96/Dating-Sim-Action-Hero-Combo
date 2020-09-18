using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStarter : GameStateListener
{
    [SerializeField] private string _transitionId;

    protected override void OnGameStateEntered() {
        GameManager.Instance.StartGame();
        GameStateManager.Instance.HandleTransition(_transitionId);
    }
}
