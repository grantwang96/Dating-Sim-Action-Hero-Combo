using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_RangedAttack : AIState_Attack
{
    public override void Execute() {
        // fail if there is no move controller or target
        if (_moveController == null || _targetManager.CurrentTarget == null) {
            OnLostTarget();
            return;
        }
        base.Execute();
        if (!CanAttack()) {
            OnLostTarget();
            return;
        }
        Attack();
        base.Execute();
    }

    protected override void OnLostTarget() {
        _navigator.LookTarget = null;
        base.OnLostTarget();
    }

    public override bool CanAttack() {
        return _targetManager.ScanForTarget(_targetManager.CurrentTarget);
    }
}
