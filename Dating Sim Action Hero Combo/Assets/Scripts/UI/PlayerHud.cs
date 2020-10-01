using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHud : UIObject
{
    [SerializeField] private FillBar _healthBar;
    [SerializeField] private FillBar _ammoBar;
    [SerializeField] private GameObject _outfitChangeInProgressDisplay;
    [SerializeField] private GameObject _agentModeActiveDisplay;
    [SerializeField] private GameObject _civilianModeDisplay;

    [SerializeField] private int _currentHealth;
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _ammo;
    [SerializeField] private int _fullClip;
    
    public override bool Initialize() {
        _currentHealth = PlayerUnit.Instance.Damageable.Health;
        _maxHealth = PlayerUnit.Instance.Damageable.MaxHealth;
        _healthBar.UpdateValue((float)_currentHealth / _maxHealth);

        _ammo = PlayerCombatController.Instance.EquippedWeapon.CurrentClip;
        _fullClip = PlayerCombatController.Instance.EquippedWeapon.Data.ClipSize;
        return base.Initialize();
    }
    
    private void OnHealthChanged(int newHealth) {
        _currentHealth = newHealth;
        _healthBar.UpdateValue((float)_currentHealth / _maxHealth);
    }

    private void OnAmmoUpdated(int ammo) {
        _ammo = ammo;
        _ammoBar.UpdateValue((float)_ammo / _fullClip);
    }

    private void OnReloadStarted() {

    }

    private void OnReloadFinished() {
        _ammo = PlayerCombatController.Instance.EquippedWeapon.CurrentClip;
        _ammoBar.UpdateValueInstant((float)_ammo / _fullClip);
    }

    private void OnOutfitChangeComplete() {
        PlayerOutfitState outfitState = PlayerOutfitController.Instance.OutfitState;
        _agentModeActiveDisplay.SetActive(outfitState == PlayerOutfitState.Agent);
        _civilianModeDisplay.SetActive(outfitState == PlayerOutfitState.Civilian);
        _outfitChangeInProgressDisplay.SetActive(false);
    }

    private void OnOutfitChangeStart(PlayerOutfitState outfitState) {
        _outfitChangeInProgressDisplay.SetActive(true);
        _agentModeActiveDisplay.SetActive(false);
        _civilianModeDisplay.SetActive(false);
    }

    public override void Display() {
        base.Display();
        PlayerUnit.Instance.Damageable.OnCurrentHealthChanged += OnHealthChanged;
        PlayerOutfitController.Instance.OnOutfitChangeComplete += OnOutfitChangeComplete;
        PlayerOutfitController.Instance.OnOutfitChangeStarted += OnOutfitChangeStart;
        PlayerCombatController.Instance.OnAmmoUpdated += OnAmmoUpdated;
        PlayerCombatController.Instance.OnReloadStarted += OnReloadStarted;
        PlayerCombatController.Instance.OnReloadFinished += OnReloadFinished;
        OnHealthChanged(PlayerUnit.Instance.Damageable.Health);
        OnAmmoUpdated(PlayerCombatController.Instance.EquippedWeapon.CurrentClip);
        OnOutfitChangeComplete();
        gameObject.SetActive(true);
    }

    public override void Hide() {
        base.Hide();
        PlayerUnit.Instance.Damageable.OnCurrentHealthChanged -= OnHealthChanged;
        PlayerOutfitController.Instance.OnOutfitChangeComplete -= OnOutfitChangeComplete;
        PlayerOutfitController.Instance.OnOutfitChangeStarted -= OnOutfitChangeStart;
        PlayerCombatController.Instance.OnAmmoUpdated -= OnAmmoUpdated;
        PlayerCombatController.Instance.OnReloadStarted -= OnReloadStarted;
        PlayerCombatController.Instance.OnReloadFinished -= OnReloadFinished;
        gameObject.SetActive(false);
    }
}
