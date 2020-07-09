using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Weapon
{
    public WeaponData Data { get; protected set; }
    public int CurrentClip { get; protected set; }
    public int TotalAmmo { get; protected set; }

    protected ActiveWeaponState _weaponState;
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

    public virtual void Use(ActivateTime time, Unit unit) {
        // if we're out of ammo, give up
        if(TotalAmmo == 0 && CurrentClip == 0) {
            return;
        }
        
        // wait until reloading is finished
        if (_isReloading) {
            return;
        }

        // try to use the weapon
        if (Data.TryActivate(time, unit, _weaponState)) {
            CurrentClip = Mathf.Max(CurrentClip - Data.AmmoPerUse, 0);
        }

        // if we've finished the clip
        if (CurrentClip == 0 && !_isReloading) {
            Reload();
        }
    }

    public void Reload() {
        if (_isReloading || CurrentClip == Data.ClipSize) {
            return;
        }
        _isReloading = true;
        _startReloadTime = Time.time;
        MonoBehaviourMaster.Instance.OnUpdate += OnUpdate;
        OnReloadStart?.Invoke();
    }

    private void OnUpdate() {
        if (IsReloadingComplete()) {
            ReloadFinished();
        }
    }

    private void ReloadFinished() {
        _isReloading = false;
        CurrentClip = TotalAmmo > Data.ClipSize ? Data.ClipSize : TotalAmmo;
        TotalAmmo -= CurrentClip;
        MonoBehaviourMaster.Instance.OnUpdate -= OnUpdate;
        OnReloadFinish?.Invoke();
    }

    private bool IsReloadingComplete() {
        return (Time.time - _startReloadTime >= Data.ReloadTime);
    }
}