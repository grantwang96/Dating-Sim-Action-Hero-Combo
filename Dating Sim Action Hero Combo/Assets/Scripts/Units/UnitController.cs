using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitController
{
    public string UnitId { get; protected set; }
    public int Health { get; protected set; }
    public Unit Unit { get; protected set; }
    public UnitData Data { get; protected set; }

    public Weapon EquippedWeapon { get; protected set; }
    public IntVector3 MapPosition => Unit.MoveController.MapPosition;
    public IntVector3 MapSpaceTarget { get; set; }
    
    public event Action<int> OnHealthChanged;
    public event Action<UnitController> OnUnitDefeated;

    public UnitController(string unitId = "") {
        UnitId = unitId;
    }

    public virtual void Dispose() {
        UnsubscribeToEvents();
    }

    protected virtual void SubscribeToEvents() {
        Unit.OnTakeDamage += OnTakeDamage;
        Unit.OnHealDamage += OnHealDamage;
    }

    protected virtual void UnsubscribeToEvents() {
        Unit.OnTakeDamage -= OnTakeDamage;
        Unit.OnHealDamage -= OnHealDamage;
    }

    #region LISTENERS

    protected virtual void OnTakeDamage(int damage, DamageType damageType, Unit attacker) {
        int totalDamage = damage;
        // if this unit is resistant to this damage
        if ((Data.Resistances & damageType) != 0) {
            totalDamage /= 4;
            totalDamage = Mathf.Max(totalDamage, 1);
        }
        // update health and states
        Health -= totalDamage;
        OnHealthChanged?.Invoke(-totalDamage);
        if (Health <= 0) {
            UnitsManager.Instance.DeregisterUnit(Unit);
            UnitDefeated();
        }
    }

    protected void FireHealthChanged(int change) {
        OnHealthChanged?.Invoke(change);
    }

    protected virtual void OnHealDamage(int damage) {
        Health += damage;
        OnHealthChanged?.Invoke(damage);
    }

    protected virtual void UnitDefeated() {
        OnUnitDefeated?.Invoke(this);
    }

    #endregion
}
