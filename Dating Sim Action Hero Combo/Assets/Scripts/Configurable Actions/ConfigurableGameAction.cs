using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class ConfigurableGameAction : ScriptableObject
{
    public abstract IConfiguredGameActionState CreateActionState();
}

public interface IConfiguredGameActionState {

    event Action OnComplete;

    void Execute();
}
