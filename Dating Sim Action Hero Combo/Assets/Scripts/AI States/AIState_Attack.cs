using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIState_Attack : AIState
{
    [SerializeField] protected NPCNavigator _navigator;
    [SerializeField] protected NPCCombatController _combatController;
    [SerializeField] protected NPCMoveController _moveController;
    [SerializeField] protected NPCTargetManager _targetManager;
    [SerializeField] protected AIState _onLostTargetState;

    public override void Enter(AIStateInitializationData initData = null) {
        // if there is not target present (ex. heard a noise)
        if(_targetManager.CurrentTarget == null) {
            OnLostTarget();
            return;
        }
        _navigator.ClearDestination();
        _navigator.LookTarget = _targetManager.CurrentTarget.transform;
        base.Enter(initData);
    }
    
    public virtual bool CanAttack() {
        return true;
    }

    protected virtual void Attack() {
        _combatController.EquippedWeapon.Use(_combatController.EquippedWeapon.Data.ActivateTime, _unit);
    }

    protected virtual void OnLostTarget() {
        SetReadyToTransition(_onLostTargetState);
    }
}
