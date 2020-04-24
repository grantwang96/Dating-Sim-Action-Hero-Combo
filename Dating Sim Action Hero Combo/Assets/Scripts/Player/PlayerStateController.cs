using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateController : UnitController {

    public static PlayerStateController Instance { get; private set; }

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
        UnitId = PlayerConfig.PlayerUnitId;
        _config = config;
        PlayerUnit.OnPlayerUnitInstanceSet += OnPlayerUnitSpawned;
    }

    private void ResetController() {
        Health = _config.MaxHealth;
        // temp until save system works
        EquippedWeapon = new Weapon(_config.EquippedWeapon);
    }

    private void OnPlayerUnitSpawned() {
        PlayerUnit.Instance.OnTakeDamage += OnTakeDamage;
        PlayerUnit.Instance.OnHealDamage += OnHealDamage;
    }

    private void OnGameEnded() {
        PlayerUnit.Instance.OnTakeDamage -= OnTakeDamage;
        PlayerUnit.Instance.OnHealDamage -= OnHealDamage;
    }

    protected override void OnTakeDamage(int damage, DamageType damageType, Unit attacker) {
        if(Health <= 0) {
            return;
        }
        int totalDamage = damage;
        // update health and states
        Health -= totalDamage;
        FireHealthChanged(-totalDamage);
        if (Health <= 0) {
            UnitDefeated();
        }
    }
}
