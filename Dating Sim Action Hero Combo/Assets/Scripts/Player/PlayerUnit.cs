using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerUnit : Unit, ITileOccupant {

    public static PlayerUnit Instance { get; private set; }

    public static event Action OnPlayerUnitInstanceSet;

    protected override void Awake() {
        base.Awake();
        Instance = this;
        OnPlayerUnitInstanceSet?.Invoke();
    }

    protected override void Start() {
        base.Start();
        UnitsManager.Instance.RegisterUnit(this);
        // temp
        UIObject playerHud = UIManager.Instance.CreateNewUIObject("prefab.ui_PlayerHud", UILayerId.HUD);
        playerHud.Display();
    }
}
