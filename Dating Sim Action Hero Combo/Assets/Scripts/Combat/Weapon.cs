using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Weapon {
    public WeaponData Data { get; protected set; }
    public int CurrentClip { get; protected set; }
    public int TotalAmmo { get; protected set; }

    protected ActiveWeaponState _weaponState;
    protected Unit _unit;
    protected bool _isReloading;
    protected float _startReloadTime;

    public event Action<int> OnCurrentClipUpdated;
    public event Action OnReloadStart;
    public event Action OnReloadFinish;

    public Weapon(Unit unit, WeaponData data, int totalAmmo) {
        _unit = unit;
        Data = data;
        CurrentClip = data.ClipSize;
        TotalAmmo = totalAmmo;
        _weaponState = new ActiveWeaponState();
    }

    public virtual void Use(ActivateTime time) {
        // if we're out of ammo, give up
        if (TotalAmmo == 0 && CurrentClip == 0) {
            return;
        }
        // wait until reloading is finished
        if (_isReloading) {
            return;
        }
        // try to use the weapon
        TryActivateWeapon(time);
        // if we've finished the clip
        if (CurrentClip == 0 && !_isReloading) {
            Reload();
        }
    }

    protected virtual void TryActivateWeapon(ActivateTime time) {
        if (Data.TryActivate(time, _unit, _weaponState)) {
            CurrentClip = Mathf.Max(CurrentClip - Data.AmmoPerUse, 0);
            OnCurrentClipUpdated?.Invoke(CurrentClip);
        }
    }

    public void Reload() {
        if (_isReloading || CurrentClip == Data.ClipSize || TotalAmmo == 0) {
            return;
        }
        _isReloading = true;
        _startReloadTime = Time.time;
        MonoBehaviourMaster.Instance.OnUpdate += ProcessReload;
        OnReloadStart?.Invoke();
    }

    protected void ProcessReload() {
        if (IsReloadingComplete()) {
            ReloadFinished();
        }
    }

    protected void ReloadFinished() {
        _isReloading = false;
        CurrentClip = TotalAmmo > Data.ClipSize ? Data.ClipSize : TotalAmmo;
        TotalAmmo -= CurrentClip;
        MonoBehaviourMaster.Instance.OnUpdate -= ProcessReload;
        OnReloadFinish?.Invoke();
    }

    protected bool IsReloadingComplete() {
        return (Time.time - _startReloadTime >= Data.ReloadTime);
    }
}

public class PlayerWeapon : Weapon {

    public PlayerWeapon(PlayerUnit unit, WeaponData weaponData, int totalAmmo) : base(unit, weaponData, totalAmmo) {
        GameEventsManager.EndGame.Subscribe(OnGameEnd);
        GameEventsManager.PauseMenu.Subscribe(OnGamePaused);
    }

    protected override void TryActivateWeapon(ActivateTime time) {
        base.TryActivateWeapon(time);
        ProcessActivateTime(time);
    }

    private void ProcessActivateTime(ActivateTime time) {
        switch (time) {
            // hook into update loop for processing
            case ActivateTime.OnHeld:
                if (!_weaponState.IsHolding) {
                    MonoBehaviourMaster.Instance.OnFixedUpdate += ProcessHeldFire;
                    _weaponState.IsHolding = true;
                }
                break;
            // unhook into update loop for processing
            case ActivateTime.OnRelease:
                if (_weaponState.IsHolding) {
                    MonoBehaviourMaster.Instance.OnFixedUpdate -= ProcessHeldFire;
                    _weaponState.IsHolding = false;
                }
                break;
        }
    }

    private void ProcessHeldFire() {
        // try to use the weapon
        TryActivateWeapon(ActivateTime.OnHeld);
    }

    private void OnGameEnd(EndGameContext endGameContext) {
        MonoBehaviourMaster.Instance.OnFixedUpdate -= ProcessHeldFire;
        _weaponState.Clear();
        GameEventsManager.EndGame.Unsubscribe(OnGameEnd);
        GameEventsManager.PauseMenu.Unsubscribe(OnGamePaused);
    }

    private void OnGamePaused(bool paused) {
        if (paused && _weaponState.IsHolding) {
            MonoBehaviourMaster.Instance.OnFixedUpdate -= ProcessHeldFire;
            _weaponState.IsHolding = false;
        }
    }
}