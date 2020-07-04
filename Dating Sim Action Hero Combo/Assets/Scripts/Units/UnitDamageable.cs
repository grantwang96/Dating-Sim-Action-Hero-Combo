using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDamageable : MonoBehaviour, IDamageable {

    [SerializeField] private Unit _unit;

    public int Health { get; private set; }
    public int MaxHealth { get; private set; }

    public event Action<int, DamageType, Unit> OnTakeDamage;
    public event Action<int> OnHealDamage;
    public event Action<int> OnCurrentHealthChanged;
    public event Action<int> OnMaxHealthChanged;
    public event Action OnDefeated;

    public void Initialize() {
        Health = _unit.UnitData.MaxHealth;
        MaxHealth = Health;
        OnCurrentHealthChanged?.Invoke(Health);
        OnMaxHealthChanged?.Invoke(MaxHealth);
    }

    public void Heal(int damage) {
        Health += damage;
        OnHealDamage?.Invoke(damage);
    }

    public void TakeDamage(int damage, DamageType damageType, Unit attacker) {
        Health -= damage;
        OnCurrentHealthChanged?.Invoke(Health);
        if(Health <= 0) {
            OnDefeated?.Invoke();
        }
        OnTakeDamage?.Invoke(damage, damageType, attacker);
    }
}
