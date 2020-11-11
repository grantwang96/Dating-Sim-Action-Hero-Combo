using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DateUnit : NPCUnit, PooledObject {

    public static DateUnit Instance { get; private set; }

    public override void Initialize(PooledObjectInitializationData initializationData) {
        Instance = this;
        base.Initialize(initializationData);
    }

    public override void Despawn() {
        gameObject.SetActive(false);
    }

    public override void Spawn() {
        gameObject.SetActive(true);
    }

    protected override void OnDefeated() {
        base.OnDefeated();
    }

    protected override void SubscribeToAllianceManager() {
        
    }

    protected override void UnsubscribeToAllianceManager() {
        
    }
}
