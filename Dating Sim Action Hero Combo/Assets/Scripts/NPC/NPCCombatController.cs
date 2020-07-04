using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCCombatController : MonoBehaviour
{
    public Weapon EquippedWeapon { get; private set; }

    public void SetWeapon(WeaponData data) {
        EquippedWeapon = new Weapon(data, data.DefaultStartAmount);
    }
}
