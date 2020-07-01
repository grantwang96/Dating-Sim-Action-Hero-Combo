using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDamageable : MonoBehaviour, IDamageable {

    public event Action<int, DamageType, Unit> OnTakeDamage;
    public event Action<int> OnHealDamage;

    public void Heal(int damage) {
        OnHealDamage?.Invoke(damage);
    }

    public void TakeDamage(int damage, DamageType damageType, Unit attacker) {
        OnTakeDamage?.Invoke(damage, damageType, attacker);
    }
}
