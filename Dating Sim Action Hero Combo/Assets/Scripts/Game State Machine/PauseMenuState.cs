using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuState : GameState
{
    protected override void OnStateEnterSuccess() {
        base.OnStateEnterSuccess();
        GameManager.Instance.TogglePauseGame();
    }
    
    protected override void ConfirmExitState() {
        GameManager.Instance.TogglePauseGame();
        base.ConfirmExitState();
    }
}

public static partial class GameEventsManager {
}
