using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_EngageHostile : AIState {

    [SerializeField] private NPCNavigator _navigator;
    [SerializeField] private NPCTargetManager _targetManager;
    [SerializeField] private AIState_Attack _attackState;
    [SerializeField] private AIState _onTargetOutOfRangeState;
    [SerializeField] private AIState _onDisarmedState;

    public override void Execute() {
        Unit target = _targetManager.CurrentTarget;
        _navigator.LookTarget = target.transform;
        if (_unit.CombatController.EquippedWeapon == null) {
            // set the controller to run away or find a new weapon
            SetReadyToTransition(_onDisarmedState);
            return;
        }
        if (_attackState.CanAttack()) {
            // set ranged attack state
            SetReadyToTransition(_attackState);
        } else {
            SetReadyToTransition(_onTargetOutOfRangeState);
        }
        base.Execute();
    }
}
