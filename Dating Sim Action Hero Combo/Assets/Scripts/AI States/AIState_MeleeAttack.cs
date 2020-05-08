using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_MeleeAttack : AIState_Attack {

    public override bool Execute() {
        // fail if there is no move controller or target
        if (_moveController == null || _target == null) {
            SetNextTransition(AIStateTransitionId.OnUnitEnemyLost);
            return true;
        }
        base.Execute();
        if (!CanAttack()) {
            SetNextTransition(AIStateTransitionId.OnUnitChase);
            return true;
        }
        Attack();
        return false;
    }

    protected override bool CanAttack() {
        IntVector3 distance = _target.MoveController.MapPosition - _unit.MoveController.MapPosition;
        return Mathf.Abs(distance.x) <= 1 && Mathf.Abs(distance.z) <= 1;
    }
}