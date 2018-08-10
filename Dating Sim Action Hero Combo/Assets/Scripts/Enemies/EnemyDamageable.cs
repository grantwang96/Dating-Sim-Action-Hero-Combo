using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageable : Damageable {

    EnemyBrain myBrain;

    private void Awake() {
        myBrain = GetComponent<EnemyBrain>();
    }

    protected override void Start() {
        base.Start();
        _health = GetComponent<EnemyMovement>().enemyData.maxHealth;
    }

    public override void TakeDamage(int damage, Transform source) {
        
        base.TakeDamage(damage, source);
        if(myBrain != null) {
            myBrain.React(source);
        }
    }

    protected override void Die() {
        base.Die();
        GameManager.Instance.grid[xPos, yPos] = null;
        gameObject.SetActive(false);

        EnemyTaskManager.Instance.EnemyKilled(this);
    }
}
