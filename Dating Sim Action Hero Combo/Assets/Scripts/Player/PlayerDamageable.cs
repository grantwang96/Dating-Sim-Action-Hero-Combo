using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageable : UnitDamageable
{
    public override void TakeDamage(int damage, DamageType damageType, Unit attacker) {
        base.TakeDamage(damage, damageType, attacker);
    }
}
