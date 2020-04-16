using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains data for player's loadout and initial start
/// </summary>
public class PlayerConfig
{
    public static PlayerConfig Instance { get; private set; }

    public WeaponData EquippedWeapon { get; private set; }

    public event Action<WeaponData> OnWeaponDataUpdated;

    public static void Create() {
        if(Instance != null) {
            CustomLogger.Error(nameof(PlayerConfig), $"Instance has already been created!");
            return;
        }
        Instance = new PlayerConfig();
    }

    public void SetEquippedWeapon(WeaponData weaponData) {
        EquippedWeapon = weaponData;
        OnWeaponDataUpdated?.Invoke(EquippedWeapon);
    }
}
