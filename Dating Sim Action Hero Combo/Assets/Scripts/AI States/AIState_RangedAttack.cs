using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_RangedAttack : AIState_Attack
{
    public override void Execute() {
        // fail if there is no move controller or target
        if (_unit.MoveController == null || _unit.TargetManager.CurrentTarget == null) {
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
        _unit.Navigator.LookTarget = null;
        base.OnLostTarget();
    }

    public override bool CanAttack() {
        return _unit.TargetManager.CanSeeTarget(_unit.TargetManager.CurrentTarget);
    }
}
