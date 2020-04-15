using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon
{
    public WeaponData Data { get; private set; }
    public int CurrentClip { get; private set; }

    private ActiveWeaponState _weaponState;
    private bool _isReloading;
    private float _startReloadTime;

    public Weapon(WeaponData data) {
        Data = data;
        CurrentClip = data.ClipSize;
        _weaponState = new ActiveWeaponState();
    }

    public void Use(ActivateTime time, Unit unit, ref int totalAmmo) {
        // if we're out of ammo, give up
        if(totalAmmo == 0 && CurrentClip == 0) {
            return;
        }

        // if we've finished the clip
        if(CurrentClip == 0 && !_isReloading) {
            _isReloading = true;
            _startReloadTime = Time.time;
        }
        
        // wait until reloading is finished
        if (!IsReloadingComplete()) {
            return;
        }

        // if finished reloading, load new clip
        if (_isReloading) {
            _isReloading = false;
            CurrentClip = totalAmmo > Data.ClipSize ? Data.ClipSize : totalAmmo;
            totalAmmo -= CurrentClip;
        }

        // try to use the weapon
        if (Data.TryActivate(time, unit, _weaponState)) {
            CurrentClip--;
        }
    }

    private bool IsReloadingComplete() {
        return (Time.time - _startReloadTime >= Data.ReloadTime);
    }
}