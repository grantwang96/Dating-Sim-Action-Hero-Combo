using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageable : Damageable {

    public static PlayerDamageable Instance;

    private void Awake() {
        Instance = this;
    }

    protected override void Start() {
        // player is not registered with typical obstructions
    }

    public override void TakeDamage(int damage, Vector2 sourcePoint) {
        _health -= damage;
        
        if(_health <= 0) {
            Die();
        }
    }

    protected override void Die() {
        base.Die();
    }
}
