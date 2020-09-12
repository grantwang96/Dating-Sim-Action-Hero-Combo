using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerUnit : Unit, ITileOccupant {

    private const string PlayerHudId = "PlayerHud";

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
        UIObject playerHud = UIManager.Instance.CreateNewUIObject(PlayerHudId, UILayerId.HUD);
        playerHud.Initialize();
        playerHud.Display();
    }
}
