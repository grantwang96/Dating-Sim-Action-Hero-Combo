using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_MeleeAttack : AIState_Attack {

    [SerializeField] private AIState _onTargetOutOfRange;

    public override void Execute() {
        // fail if there is no move controller or target
        if (_moveController == null || _target == null) {
            OnLostTarget();
            return;
        }
        base.Execute();
        if (!CanAttack()) {
            SetReadyToTransition(_onTargetOutOfRange);
            return;
        }
        Attack();
    }

    protected override bool CanAttack() {
        IntVector3 distance = _target.MoveController.MapPosition - _unit.MoveController.MapPosition;
        return Mathf.Abs(distance.x) <= 1 && Mathf.Abs(distance.z) <= 1;
    }
}