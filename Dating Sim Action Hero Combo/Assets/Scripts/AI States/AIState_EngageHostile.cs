using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_EngageHostile : AIState
{
    [SerializeField] private AIState_Attack _attackState;
    [SerializeField] private AIState _onTargetOutOfRangeState;
    [SerializeField] private AIState _onDisarmedState;

    public override void Execute() {
        Unit target = _unit.TargetManager.CurrentTarget;
        if(target == null) {
            SetReadyToTransition(_onTargetOutOfRangeState);
            return;
        }
        _unit.Navigator.LookTarget = target.transform;
        if (_unit.CombatController.EquippedWeapon == null) {
            // set the controller to run away or find a new weapon
            SetReadyToTransition(_onDisarmedState);
            return;
        }
        if (_attackState.CanAttack()) {
            // set attack state
            SetReadyToTransition(_attackState);
        } else {
            SetReadyToTransition(_onTargetOutOfRangeState);
        }
        base.Execute();
    }
}

public class AIStateEngageHostileInitData : AIStateInitializationData {

    public AIStateEngageHostileInitData(NPCUnit unit) {

    }
}
