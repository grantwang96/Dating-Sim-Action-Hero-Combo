using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuState : GameState
{
    protected override void OnStateEnterSuccess() {
        base.OnStateEnterSuccess();
        // unhook listeners from input system
        // disable processing for enemy AI
        // stop the time
        // ???
    }

    public override void Exit(GameState nextState) {
        base.Exit(nextState);
        // rehook listeners to input system
        // enable processing for enemy AI
        // turn time back on
        // ???
    }
}
