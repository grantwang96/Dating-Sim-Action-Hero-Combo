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
        _health = myBrain.MyBluePrint.maxHealth;
    }

    public override void TakeDamage(int damage, Damageable source) {
        
        base.TakeDamage(damage, source);
        if(myBrain != null) {
            Damageable dam = source.GetComponent<Damageable>();
            myBrain.ReactToThreat(source);
        }
    }

    protected override void Die() {
        base.Die();
        GameManager.Instance.grid[xPos, yPos] = null;
        gameObject.SetActive(false);

        EnemyTaskManager.Instance.EnemyKilled(this);
    }
}
