using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_RangedAttack : AIState_Attack
{
    [SerializeField] private NPCTargetManager _npcTargetManager;

    public override void Execute() {
        // fail if there is no move controller or target
        if (_moveController == null || _target == null) {
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
        _moveController.SetLookTarget(null);
        base.OnLostTarget();
    }

    protected override bool CanAttack() {
        return _npcTargetManager.ScanForHostile(_npcTargetManager.CurrentTarget);
    }
}
