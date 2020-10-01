using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DateUnit : NPCUnit, PooledObject {

    public static DateUnit Instance { get; private set; }

    public void Initialize(PooledObjectInitializationData initializationData) {

    }

    public override void Initialize(string unitId, UnitData data) {
        Instance = this;
        base.Initialize(unitId, data);
    }

    public void Despawn() {
        gameObject.SetActive(false);
        PooledObjectManager.Instance.ReturnPooledObject(this.name, this);
    }

    public void Spawn() {
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
