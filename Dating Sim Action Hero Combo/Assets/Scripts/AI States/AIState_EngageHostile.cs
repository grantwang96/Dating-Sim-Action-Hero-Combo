using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_EngageHostile : AIState {

    [SerializeField] private AIState _rangedAttackState;
    [SerializeField] private AIState _meleeAttackState;
    [SerializeField] private AIState _onDisarmedState;

    public override void Execute() {
        /*
        if (_controller.EquippedWeapon == null) {
            // set the controller to run away or find a new weapon
            SetReadyToTransition(_onDisarmedState);
            return;
        }
        if (_controller.EquippedWeapon.Data.IsRanged) {
            // set ranged attack state
            SetReadyToTransition(_rangedAttackState);
        } else {
            // set melee attack state
            SetReadyToTransition(_meleeAttackState);
        }
        */
        base.Execute();
    }
}
