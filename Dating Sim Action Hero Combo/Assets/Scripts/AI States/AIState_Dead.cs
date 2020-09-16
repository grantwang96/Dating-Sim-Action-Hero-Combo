using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_Dead : AIState {

    [SerializeField] private Collider2D _collider;

    public override void Enter(AIStateInitializationData initData = null) {
        base.Enter(initData);
        _collider.enabled = false;
    }
    
    public override void Exit(AIState nextState) {
        base.Exit(nextState);
        _collider.enabled = true;
    }
}
