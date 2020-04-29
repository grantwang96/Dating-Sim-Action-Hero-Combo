using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerCombatController : PlayerActionController
{
    public static PlayerCombatController Instance { get; private set; }

    // store currently held weapon
    public Weapon EquippedWeapon { get; private set; }

    public event Action<int> OnAmmoUpdated;
    public event Action OnReloadStarted;
    public event Action OnReloadFinished;

    public PlayerCombatController(PlayerUnit unit) : base(unit) {
        Instance = this;
        UpdateEquippedWeapon(PlayerStateController.Instance.EquippedWeapon);
    }

    private void UpdateEquippedWeapon(Weapon weapon) {
        if(EquippedWeapon != null) {
            weapon.OnReloadStart -= ReloadStarted;
            weapon.OnReloadFinish -= ReloadFinished;
        }
        EquippedWeapon = weapon;
        EquippedWeapon.OnReloadStart += ReloadStarted;
        EquippedWeapon.OnReloadFinish += ReloadFinished;
    }

    protected override void SubscribeToEvents() {
        base.SubscribeToEvents();

        InputController.Instance.ShootBtnPressed += OnShootBtnPressed;
        InputController.Instance.ShootBtnHeld += OnShootBtnHeld;
        InputController.Instance.ShootBtnReleased += OnShootBtnReleased;
    }

    protected override void UnsubscribeToEvents() {
        base.UnsubscribeToEvents();

        InputController.Instance.ShootBtnPressed -= OnShootBtnPressed;
        InputController.Instance.ShootBtnHeld -= OnShootBtnHeld;
        InputController.Instance.ShootBtnReleased -= OnShootBtnReleased;
    }

    private void OnShootBtnPressed() {
        EquippedWeapon.Use(ActivateTime.OnPress, PlayerUnit.Instance);
        OnAmmoUpdated?.Invoke(EquippedWeapon.CurrentClip);
    }

    private void OnShootBtnHeld() {
        EquippedWeapon.Use(ActivateTime.OnHeld, PlayerUnit.Instance);
        OnAmmoUpdated?.Invoke(EquippedWeapon.CurrentClip);
    }

    private void OnShootBtnReleased() {
        EquippedWeapon.Use(ActivateTime.OnRelease, PlayerUnit.Instance);
        OnAmmoUpdated?.Invoke(EquippedWeapon.CurrentClip);
    }

    private void ReloadStarted() {
        OnReloadStarted?.Invoke();
    }

    private void ReloadFinished() {
        OnReloadFinished?.Invoke();
    }
}
