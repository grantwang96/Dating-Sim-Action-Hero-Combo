using System.Collections;
using System.Collections.Generic;
using System;

public interface IDamageable : IUnitComponent
{
    int Health { get; }
    int MaxHealth { get; }

    event Action<int, DamageType, Unit> OnTakeDamage;
    event Action<int> OnHealDamage;
    event Action<int> OnCurrentHealthChanged;
    event Action<int> OnMaxHealthChanged;
    event Action OnDefeated;

    void TakeDamage(int damage, DamageType damageType, Unit attacker);
    void Heal(int damage);
}

[Flags]
public enum DamageType {
    Normal,
    Explosive,
    Chemical
}
