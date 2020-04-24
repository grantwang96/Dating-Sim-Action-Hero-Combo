using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_EngageHostile : AIState {

    public override bool Execute() {
        if (_controller.EquippedWeapon == null) {
            // set the controller to run away or find a new weapon
            SetNextTransition(AIStateTransitionId.OnUnitDefeated);
            return true;
        }
        if (_controller.EquippedWeapon.Data.IsRanged) {
            // set ranged attack state
            SetNextTransition(AIStateTransitionId.OnUnitRangedAttack);
        } else {
            // set melee attack state
            SetNextTransition(AIStateTransitionId.OnUnitMeleeAttack);
        }
        return base.Execute();
    }
}
