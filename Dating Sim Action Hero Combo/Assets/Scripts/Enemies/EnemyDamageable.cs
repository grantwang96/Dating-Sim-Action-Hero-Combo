using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageable : Damageable {

    public override void TakeDamage(int damage, Vector2 sourcePoint) {
        
        base.TakeDamage(damage, sourcePoint);
    }

    protected override void Die() {
        base.Die();
        GameManager.Instance.grid[xPos, yPos] = null;
        gameObject.SetActive(false);
    }
}
