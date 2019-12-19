using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    // store currently held weapon
    private WeaponSlot _equippedWeapon;
    [SerializeField] private int _hackBulletCount;
    [SerializeField] private WeaponData _hackWeaponData; // hack

    private void Start() {
        _equippedWeapon = new WeaponSlot(_hackWeaponData);
        SubscribeToController();
    }

    private void OnDisable() {
        UnsubscribeToController();
    }

    private void OnEnable() {
        if (InputController.Instance != null) {
            SubscribeToController();
        }
    }

    private void SubscribeToController() {
        UnsubscribeToController(); // ensure we don't double subscribe
        InputController.Instance.ShootBtnPressed += OnShootBtnPressed;
        InputController.Instance.ShootBtnHeld += OnShootBtnHeld;
        InputController.Instance.ShootBtnReleased += OnShootBtnReleased;
    }

    private void UnsubscribeToController() {
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
