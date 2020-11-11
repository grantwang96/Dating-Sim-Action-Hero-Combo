using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyUnit : NPCUnit {

    public override void Spawn() {
        gameObject.SetActive(true);
        SubscribeToAllianceManager();
    }

    public override void Despawn() {
        gameObject.SetActive(false);
        UnsubscribeToAllianceManager();
    }

    protected override void OnDefeated() {
        UnsubscribeToAllianceManager();
        base.OnDefeated();
    }

    protected override void SubscribeToAllianceManager() {
        EnemyManager.Instance.OnAllianceMessageSent += OnAllianceMessageSent;
    }

    protected override void UnsubscribeToAllianceManager() {
        EnemyManager.Instance.OnAllianceMessageSent -= OnAllianceMessageSent;
    }
}
