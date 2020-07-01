using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyUnit : NPCUnit, PooledObject {

    public void Despawn() {
        gameObject.SetActive(false);
        PooledObjectManager.Instance.ReturnPooledObject(this.name, this);
    }

    public void Spawn() {
        gameObject.SetActive(true);
    }
}
