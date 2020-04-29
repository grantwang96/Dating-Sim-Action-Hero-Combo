using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Weapon
{
    public WeaponData Data { get; private set; }
    public int CurrentClip { get; private set; }
    public int TotalAmmo { get; private set; }

    private ActiveWeaponState _weaponState;
    private bool _isReloading;
    private float _startReloadTime;

    public event Action OnReloadStart;
    public event Action OnReloadFinish;

    public Weapon(WeaponData data, int totalAmmo) {
        Data = data;
        CurrentClip = data.ClipSize;
        TotalAmmo = totalAmmo;
        _weaponState = new ActiveWeaponState();
    }

    public void Use(ActivateTime time, Unit unit) {
        // if we're out of ammo, give up
        if(TotalAmmo == 0 && CurrentClip == 0) {
            return;
        }
        
        // wait until reloading is finished
        if (!IsReloadingComplete()) {
            return;
        }

        // try to use the weapon
        if (Data.TryActivate(time, unit, _weaponState)) {
            CurrentClip--;
        }

        // if we've finished the clip
        if (CurrentClip == 0 && !_isReloading) {
            _isReloading = true;
            _startReloadTime = Time.time;
            MonoBehaviourMaster.Instance.OnUpdate += OnUpdate;
            OnReloadStart?.Invoke();
        }
    }

    private void OnUpdate() {
        if (IsReloadingComplete()) {
            ReloadFinished();
            MonoBehaviourMaster.Instance.OnUpdate -= OnUpdate;
        }
    }

    private void ReloadFinished() {
        _isReloading = false;
        CurrentClip = TotalAmmo > Data.ClipSize ? Data.ClipSize : TotalAmmo;
        TotalAmmo -= CurrentClip;
        OnReloadFinish?.Invoke();
    }

    private bool IsReloadingComplete() {
        return (Time.time - _startReloadTime >= Data.ReloadTime);
    }
}