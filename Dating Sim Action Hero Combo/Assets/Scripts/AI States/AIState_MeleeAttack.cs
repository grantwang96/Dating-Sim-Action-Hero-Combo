using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI State/Melee Attack")]
public class AIState_MeleeAttack : AIStateDataObject {

    protected override ActiveAIState GenerateActiveAIState(NPCUnitController controller) {
        ActiveAIState aiState = new ActiveMeleeState(controller);
        return aiState;
    }
}

public class ActiveMeleeState : ActiveAttackState {
    
    public ActiveMeleeState(NPCUnitController controller) : base(controller) {

    }

    public override bool OnExecute() {
        // fail if there is no move controller or target
        if (_moveController == null || _target == null) {
            SetNextTransition(AIStateTransitionId.OnUnitEnemyLost);
            return true;
        }
        base.OnExecute();
        if(!CanAttack()) {
            SetNextTransition(AIStateTransitionId.OnUnitChase);
            return true;
        }
        Attack();
        return false;
    }

    protected override bool CanAttack() {
        IntVector3 distance = _target.MoveController.MapPosition - _unit.MoveController.MapPosition;
        return Mathf.Abs(distance.x) <= 1 && Mathf.Abs(distance.z) <= 1;
    }
}