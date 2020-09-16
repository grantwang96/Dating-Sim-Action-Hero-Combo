using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuState : GameState
{
    protected override void OnStateEnterSuccess() {
        base.OnStateEnterSuccess();
        // unhook listeners from input system
        GameEventsManager.Pause?.Broadcast(true);
        // disable processing for enemy AI
        // stop the time
        // ???
    }

    public override void Exit(GameState nextState) {
        base.Exit(nextState);
        // rehook listeners to input system
        GameEventsManager.Pause?.Broadcast(false);
        // enable processing for enemy AI
        // turn time back on
        // ???
    }
}

public static partial class GameEventsManager {
    public static GameEvent<bool> Pause { get; } = new GameEvent<bool>();
}
