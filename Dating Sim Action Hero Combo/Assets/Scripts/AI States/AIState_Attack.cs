﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIState_Attack : AIState
{
    [SerializeField] protected AIState _onLostTargetState;
    
    protected override void OnEnter() {
        base.OnEnter();
        // if there is not target present (ex. heard a noise)
        if (_unit.TargetManager.CurrentTarget == null) {
            OnLostTarget();
            return;
        }
        _unit.Navigator.ClearDestination();
        _unit.Navigator.LookTarget = _unit.TargetManager.CurrentTarget.transform;
    }

    public virtual bool CanAttack() {
        return true;
    }

    protected virtual void Attack() {
        _unit.CombatController.UseWeapon(_unit.CombatController.EquippedWeapon.Data.ActivateTime, _unit);
    }

    protected virtual void OnLostTarget() {
        SetReadyToTransition(_onLostTargetState);
    }
}
