using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI State/Engage Hostile")]
public class AIState_EngageHostile : AIStateDataObject {

    protected override ActiveAIState GenerateActiveAIState(IUnitController controller) {
        ActiveEngageHostileState activeState = new ActiveEngageHostileState(controller);
        return activeState;
    }
}

public class ActiveEngageHostileState : ActiveAIState {

    private IUnitController _controller;
    private WeaponSlot _equippedWeapon;

    public ActiveEngageHostileState(IUnitController controller) {
        _controller = controller;
        _equippedWeapon = controller.EquippedWeapon;
    }

    public override bool OnExecute() {
        if (_equippedWeapon == null) {
            // set the controller to run away or find a new weapon
            SetNextTransition(AIStateTransitionId.OnUnitDefeated);
            return true;
        }
        if (_equippedWeapon.Data.IsRanged) {
            // set ranged attack state
            SetNextTransition(AIStateTransitionId.OnUnitRangedAttack);
        } else {
            // set melee attack state
            SetNextTransition(AIStateTransitionId.OnUnitMeleeAttack);
        }
        return true;
    }
}
