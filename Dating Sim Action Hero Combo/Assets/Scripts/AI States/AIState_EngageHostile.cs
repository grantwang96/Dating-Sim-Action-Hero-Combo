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

    public ActiveEngageHostileState(IUnitController controller) {
        _controller = controller;
        WeaponSlot equippedWeapon = controller.EquippedWeapon;
        if(equippedWeapon == null) {
            // set the controller to run away or find a new weapon
            SetNextTransition(AIStateTransitionId.OnUnitDefeated);
            return;
        }
        if (equippedWeapon.Data.IsRanged) {
            // set ranged attack state
        } else {
            // set melee attack state
        }
    }
}
