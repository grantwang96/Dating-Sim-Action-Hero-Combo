using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_Dead : AIState {

    [SerializeField] private Collider2D _collider;
    
    protected override void OnEnter() {
        base.OnEnter();
        _collider.enabled = false;
    }

    protected override void OnExit() {
        base.OnExit();
        _collider.enabled = true;
    }
}
