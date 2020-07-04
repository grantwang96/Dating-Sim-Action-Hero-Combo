using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_MeleeAttack : AIState_Attack {

    [SerializeField] private AIState _onTargetOutOfRange;

    public override void Execute() {
        // fail if there is no move controller or target
        if (_moveController == null || _targetManager.CurrentTarget == null) {
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

    public override bool CanAttack() {
        IntVector3 distance = _targetManager.CurrentTarget.MoveController.MapPosition - _unit.MoveController.MapPosition;
        return Mathf.Abs(distance.x) <= 1 && Mathf.Abs(distance.y) <= 1;
    }
}