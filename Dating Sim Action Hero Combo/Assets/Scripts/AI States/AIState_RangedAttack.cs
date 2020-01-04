using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI State/Ranged Attack")]
public class AIState_RangedAttack : AIStateDataObject
{
    protected override ActiveAIState GenerateActiveAIState(IUnitController controller) {
        ActiveRangedAttackState rangedAttackState = new ActiveRangedAttackState(controller);
        return rangedAttackState;
    }
}

public class ActiveRangedAttackState : ActiveAttackState {
 
    public ActiveRangedAttackState(IUnitController controller) : base(controller) {

    }

    // attempt to fire weapon continuously
    public override bool OnExecute() {
        // fail if there is no move controller or target
        if(_moveController == null || _target == null) {
            SetNextTransition(AIStateTransitionId.OnUnitEnemyLost);
            return true;
        }
        base.OnExecute();
        if (!CanAttack()) {
            LostTarget();
            return true;
        }
        Attack();
        return false;
    }

    protected override bool CanAttack() {
        return ActiveScanState.Scan(_target, _unit.transform, _unitData.VisionRange, _unitData.VisionLayers);
    }

    private void LostTarget() {
        _moveController.SetLookTarget(null);
        SetNextTransition(AIStateTransitionId.OnUnitEnemyLost);
    }
}
