using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIState_Attack : AIState
{
    [SerializeField] protected NPCMoveController _moveController;

    protected Unit _target;
    protected UnitData _unitData;
    protected Weapon _equippedWeapon;

    public override void Enter(AIStateInitializationData initData = null) {
        _unitData = _controller.Data;
        // if there is not target present (ex. heard a noise)
        if(_controller.FocusedTarget == null) {
            SetNextTransition(AIStateTransitionId.OnUnitChase);
            return;
        }
        _target = _controller.FocusedTarget;
        _equippedWeapon = _controller.EquippedWeapon;
        _moveController.ClearDestination();
        _moveController.SetLookTarget(_target.transform);
        base.Enter(initData);
    }
    
    protected virtual bool CanAttack() {
        return true;
    }

    protected virtual void Attack() {
        _equippedWeapon.Use(_equippedWeapon.Data.ActivateTime, _unit);
    }
}
