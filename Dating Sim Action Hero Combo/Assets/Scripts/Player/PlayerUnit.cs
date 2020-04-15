using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : Unit, ITileOccupant {

    public static PlayerUnit Instance { get; private set; }

    [SerializeField] private Transform _front;
    [SerializeField] private PlayerMoveController _playerMoveController;
    public override MoveController MoveController => _playerMoveController;

    public override Transform Transform => transform;
    public override Transform Front => _front;

    [SerializeField] private float _speed;

    // temp
    public WeaponData HackPlayerWeaponConfig;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        UnitsManager.Instance.RegisterUnit(this);
    }

    public override void TakeDamage(int damage, DamageType damageType, Unit attacker) {
        // visual response here
        base.TakeDamage(damage, damageType, attacker);
    }
}
