using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyData")]
public class EnemyData : UnitData
{
    [SerializeField] private WeaponData _equippedWeapon;
    public WeaponData EquippedWeapon => _equippedWeapon;
}
