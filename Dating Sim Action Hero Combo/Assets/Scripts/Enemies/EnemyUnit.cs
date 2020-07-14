using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyUnit : NPCUnit, PooledObject {

    public void Despawn() {
        gameObject.SetActive(false);
        PooledObjectManager.Instance.ReturnPooledObject(this.name, this);
        EnemyManager.Instance.OnAllianceMessageSent -= OnAllianceMessageSent;
    }

    public void Spawn() {
        gameObject.SetActive(true);
        EnemyManager.Instance.OnAllianceMessageSent += OnAllianceMessageSent;
    }

    protected override void SubscribeToAllianceManager() {
        EnemyManager.Instance.OnAllianceMessageSent += OnAllianceMessageSent;
    }

    protected override void UnsubscribeToAllianceManager() {
        EnemyManager.Instance.OnAllianceMessageSent -= OnAllianceMessageSent;
    }
}
