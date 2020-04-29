using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains data for player's loadout and initial start. This class will likely be dissolved into several different classes that will be pulled from save data. 
/// </summary>
[CreateAssetMenu(menuName = "PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
    public const string PlayerUnitId = "PLAYER_UNIT";

    [SerializeField] private int _maxHealth;
    public int MaxHealth => _maxHealth;
    [SerializeField] private int _totalAmmoClips;
    public int TotalAmmoClips => _totalAmmoClips;
    [SerializeField] private WeaponData _currentLoadout;
    public WeaponData CurrentLoadout => _currentLoadout;
}
