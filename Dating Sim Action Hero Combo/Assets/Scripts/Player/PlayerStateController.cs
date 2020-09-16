using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateController {

    public static PlayerStateController Instance { get; private set; }
    public WeaponData EquippedWeapon => _config.CurrentLoadout;
    public int StartingAmmoClips => _config.TotalAmmoClips;
    public int StartingAmmo => StartingAmmoClips * EquippedWeapon.ClipSize;

    private PlayerConfig _config;

    public event Action<int> OnHealthChanged;

    public static void Create(PlayerConfig config) {
        PlayerStateController controller = new PlayerStateController(config);
        Instance = controller;
        Reset();
    }

    public static void Reset() {
        Instance?.ResetController();
    }

    private PlayerStateController(PlayerConfig config) {
        // TODO: create save system to persist/retrieve information about current loadout and state before entering a combat level
        _config = config;
        PlayerUnit.OnPlayerUnitInstanceSet += OnPlayerUnitSpawned;
    }

    private void ResetController() {

    }

    private void OnPlayerUnitSpawned() {
        PlayerUnit.Instance.Initialize(PlayerConfig.PlayerUnitId, _config.UnitData);
    }

    private void OnGameEnded() {
        PlayerUnit.Instance.Dispose();
    }
}
