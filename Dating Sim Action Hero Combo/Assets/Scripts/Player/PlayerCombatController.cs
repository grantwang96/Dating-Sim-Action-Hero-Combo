using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : PlayerActionController
{
    // store currently held weapon
    private Weapon _equippedWeapon;

    private static int _hackBulletCount = 800;

    public PlayerCombatController(PlayerUnit unit) : base(unit) {
        _equippedWeapon = new Weapon(unit.HackPlayerWeaponConfig);
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
        _equippedWeapon.Use(ActivateTime.OnPress, PlayerUnit.Instance, ref _hackBulletCount);
    }

    private void OnShootBtnHeld() {
        _equippedWeapon.Use(ActivateTime.OnHeld, PlayerUnit.Instance, ref _hackBulletCount);
    }

    private void OnShootBtnReleased() {
        _equippedWeapon.Use(ActivateTime.OnRelease, PlayerUnit.Instance, ref _hackBulletCount);
    }
}
