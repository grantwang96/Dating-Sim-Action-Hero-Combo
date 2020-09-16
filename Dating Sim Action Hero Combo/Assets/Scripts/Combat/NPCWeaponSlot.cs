using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCWeaponSlot : Weapon
{
    public NPCWeaponSlot(Unit unit, WeaponData data, int totalAmmo) : base(unit, data, totalAmmo) {
        Data = data;
        CurrentClip = data.ClipSize;
        TotalAmmo = totalAmmo;
        _weaponState = new ActiveWeaponState();
    }

    public override void Use(ActivateTime time) {
        base.Use(time);
        if (TotalAmmo == 0 && CurrentClip == 0) {
            TotalAmmo = Data.DefaultStartAmount;
            CurrentClip = Data.ClipSize;
        }
    }
}
