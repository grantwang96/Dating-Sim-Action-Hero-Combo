using System.Collections;
using System.Collections.Generic;
using System;

public interface IDamageable
{
    event Action<int, DamageType, Unit> OnTakeDamage;
    event Action<int> OnHealDamage;

    void TakeDamage(int damage, DamageType damageType, Unit attacker);
    void Heal(int damage);
}

[Flags]
public enum DamageType {
    Normal,
    Explosive,
    Chemical
}
