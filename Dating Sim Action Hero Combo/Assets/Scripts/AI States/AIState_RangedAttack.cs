using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_RangedAttack : AIState_Attack
{
    public override bool Execute() {
        // fail if there is no move controller or target
        if (_moveController == null || _target == null) {
            SetNextTransition(AIStateTransitionId.OnUnitEnemyLost);
            return true;
        }
        base.Execute();
        if (!CanAttack()) {
            LostTarget();
            return true;
        }
        Attack();
        return base.Execute();
    }

    private void LostTarget() {
        _moveController.SetLookTarget(null);
        SetNextTransition(AIStateTransitionId.OnUnitEnemyLost);
    }

    protected override bool CanAttack() {
        return AIState_Scan.Scan(_target, _unit.transform, _unitData.VisionRange, _unitData.VisionLayers);
    }
}
