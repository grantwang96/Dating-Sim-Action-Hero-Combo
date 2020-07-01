using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerUnit : Unit, ITileOccupant {

    public static PlayerUnit Instance { get; private set; }

    public static event Action OnPlayerUnitInstanceSet;

    private void Awake() {
        Instance = this;
        OnPlayerUnitInstanceSet?.Invoke();
    }

    private void Start() {
        UnitsManager.Instance.RegisterUnit(this);
        // temp
        UIObject playerHud = UIManager.Instance.CreateNewUIObject("prefab.ui_PlayerHud", UILayerId.HUD);
        playerHud.Display();
    }
}
