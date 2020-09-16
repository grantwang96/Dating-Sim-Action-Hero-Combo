using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCCombatController : MonoBehaviour
{
    [SerializeField] private NPCUnit _unit;

    public Weapon EquippedWeapon { get; private set; }

    public void SetWeapon(WeaponData data) {
        EquippedWeapon = new NPCWeaponSlot(_unit, data, data.DefaultStartAmount);
    }

    public void UseWeapon(ActivateTime activateTime, Unit unit) {
        EquippedWeapon.Use(activateTime);
    }
}
