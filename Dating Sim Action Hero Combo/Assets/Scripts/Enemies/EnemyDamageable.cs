using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageable : Damageable {

    protected override void Die() {
        base.Die();
        Destroy(gameObject);
    }
}
