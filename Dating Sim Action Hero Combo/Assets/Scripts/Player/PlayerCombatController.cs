using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerCombatController : PlayerActionController
{
    public static PlayerCombatController Instance { get; private set; }

    // store currently held weapon
    public Weapon EquippedWeapon { get; private set; }

    public event Action<int> OnAmmoUpdated;
    public event Action OnEquippedWeaponUpdated;
    public event Action OnReloadStarted;
    public event Action OnReloadFinished;

    public PlayerCombatController(PlayerUnit unit) : base(unit) {
        Instance = this;
        PlayerWeapon weapon = new PlayerWeapon(unit, PlayerStateController.Instance.EquippedWeapon, PlayerStateController.Instance.StartingAmmo);
        EquippedWeapon = weapon;
        OnEquippedWeaponUpdated?.Invoke();
    }

    private void UpdateEquippedWeapon(PlayerWeapon weapon) {
        if(EquippedWeapon != null) {
            UnsubscribeToWeaponEvents();
        }
        EquippedWeapon = weapon;
        SubscribeToWeaponEvents();
        OnEquippedWeaponUpdated?.Invoke();
    }

    private void SubscribeToWeaponEvents() {
        if (EquippedWeapon == null) {
            return;
        }
        EquippedWeapon.OnCurrentClipUpdated += OnCurrentClipUpdated;
        EquippedWeapon.OnReloadStart += ReloadStarted;
        EquippedWeapon.OnReloadFinish += ReloadFinished;
    }

    private void UnsubscribeToWeaponEvents() {
        if (EquippedWeapon == null) {
            return;
        }
        EquippedWeapon.OnCurrentClipUpdated -= OnCurrentClipUpdated;
        EquippedWeapon.OnReloadStart -= ReloadStarted;
        EquippedWeapon.OnReloadFinish -= ReloadFinished;
    }

    protected override void SubscribeToEvents() {
        base.SubscribeToEvents();

        InputController.Instance.GameplayActionMap[InputStrings.FireKey].started += OnShootBtnPressed;
        InputController.Instance.GameplayActionMap[InputStrings.FireKey].performed += OnShootBtnHeld;
        InputController.Instance.GameplayActionMap[InputStrings.FireKey].canceled += OnShootBtnReleased;
        InputController.Instance.GameplayActionMap[InputStrings.ReloadKey].started += OnReloadBtnPressed;

        SubscribeToWeaponEvents();
    }

    protected override void UnsubscribeToEvents() {
        base.UnsubscribeToEvents();

        InputController.Instance.GameplayActionMap[InputStrings.FireKey].started -= OnShootBtnPressed;
        InputController.Instance.GameplayActionMap[InputStrings.FireKey].performed -= OnShootBtnHeld;
        InputController.Instance.GameplayActionMap[InputStrings.FireKey].canceled -= OnShootBtnReleased;

        InputController.Instance.GameplayActionMap[InputStrings.ReloadKey].started -= OnReloadBtnPressed;

        UnsubscribeToWeaponEvents();
    }
    
    private void OnShootBtnPressed(InputAction.CallbackContext context) {
        EquippedWeapon.Use(ActivateTime.OnPress);
    }

    private void OnShootBtnHeld(InputAction.CallbackContext context) {
        EquippedWeapon.Use(ActivateTime.OnHeld);
    }

    private void OnShootBtnReleased(InputAction.CallbackContext context) {
        EquippedWeapon.Use(ActivateTime.OnRelease);
    }

    private void OnReloadBtnPressed(InputAction.CallbackContext context) {
        EquippedWeapon.Reload();
    }

    private void OnCurrentClipUpdated(int currentClip) {
        OnAmmoUpdated?.Invoke(currentClip);
    }

    private void ReloadStarted() {
        OnReloadStarted?.Invoke();
    }

    private void ReloadFinished() {
        OnReloadFinished?.Invoke();
    }
}
