using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateController {

    public static PlayerStateController Instance { get; private set; }

    public Weapon EquippedWeapon { get; protected set; }
    public int StartingAmmoClips => _config.TotalAmmoClips;

    private PlayerConfig _config;

    public static void Create(PlayerConfig config) {
        PlayerStateController controller = new PlayerStateController(config);
        Instance = controller;
        Reset();
    }

    public static void Reset() {
        Instance?.ResetController();
    }

    private PlayerStateController(PlayerConfig config) {
        _config = config;
        PlayerUnit.OnPlayerUnitInstanceSet += OnPlayerUnitSpawned;
    }

    private void ResetController() {
        // temp until save system works
        EquippedWeapon = new Weapon(_config.CurrentLoadout, _config.TotalAmmoClips * _config.CurrentLoadout.ClipSize);
    }

    private void OnPlayerUnitSpawned() {
        PlayerUnit.Instance.Damageable.OnTakeDamage += OnTakeDamage;
    }

    private void OnGameEnded() {
        PlayerUnit.Instance.Damageable.OnTakeDamage -= OnTakeDamage;
    }

    protected void OnTakeDamage(int damage, DamageType damageType, Unit attacker) {
        int totalDamage = damage;
    }
}
