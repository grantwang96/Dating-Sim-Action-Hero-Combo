using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageable : Damageable {

    protected override void Start() {
        // player is not registered with typical obstructions
    }

    public override void TakeDamage(int damage) {
        _health -= damage;
        
        if(_health <= 0) {
            Die();
        }
    }

    protected override void Die() {
        base.Die();
    }
}
