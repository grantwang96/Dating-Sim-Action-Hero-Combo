using System.Collections;
using System.Collections.Generic;

public interface IDamageable
{
    int Health { get; }
    bool IsDead { get; }

    void TakeDamage(int damage, DamageType damageType);
}

[System.Flags]
public enum DamageType {
    Normal,
    Explosive,
    Chemical
}
