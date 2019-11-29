using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    // store currently held weapon
    [SerializeField] private WeaponData _equippedWeapon;
    private ActiveWeaponState _activeWeaponState = new ActiveWeaponState();

    private void Start() {
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
        _equippedWeapon.TryActivate(ActivateTime.OnPress, PlayerUnit.Instance, _activeWeaponState);
    }

    private void OnShootBtnHeld() {
        _equippedWeapon.TryActivate(ActivateTime.OnHeld, PlayerUnit.Instance, _activeWeaponState);
    }

    private void OnShootBtnReleased() {
        _equippedWeapon.TryActivate(ActivateTime.OnRelease, PlayerUnit.Instance, _activeWeaponState);
    }
}
