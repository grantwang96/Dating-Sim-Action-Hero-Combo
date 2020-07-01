using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIState_Attack : AIState
{
    [SerializeField] protected NPCMoveController _moveController;
    [SerializeField] protected NPCTargetManager _targetManager;
    [SerializeField] protected AIState _onLostTargetState;

    protected Unit _target;
    protected Weapon _equippedWeapon;

    public override void Enter(AIStateInitializationData initData = null) {
        // if there is not target present (ex. heard a noise)
        if(_targetManager.CurrentTarget == null) {
            OnLostTarget();
            return;
        }
        _target = _targetManager.CurrentTarget;
        // _equippedWeapon = _unit.UnitData;
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

    protected virtual void OnLostTarget() {
        SetReadyToTransition(_onLostTargetState);
    }
}
