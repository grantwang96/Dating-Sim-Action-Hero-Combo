using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerUnit : Unit, ITileOccupant {

    public static PlayerUnit Instance { get; private set; }

    [SerializeField] private Transform _front;
    [SerializeField] private PlayerMoveController _playerMoveController;
    public override MoveController MoveController => _playerMoveController;

    public override Transform Transform => transform;
    public override Transform Front => _front;

    [SerializeField] private float _speed;

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

    public override void TakeDamage(int damage, DamageType damageType, Unit attacker) {
        // visual response here
        base.TakeDamage(damage, damageType, attacker);
    }
}
