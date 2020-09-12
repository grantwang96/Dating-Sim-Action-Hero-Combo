using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGameState : GameState
{
    protected override IInitializableManager[] _initializableManagers => new IInitializableManager[] {
        new LoadingScreenController()
    };
}
