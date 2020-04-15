using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI State/Move")]
public class AIState_Move : AIStateDataObject
{
    [SerializeField] private bool _fullSpeed;

    protected override ActiveAIState GenerateActiveAIState(IUnitController controller) {
        float speed = _fullSpeed ? controller.Data.RunSpeed : controller.Data.WalkSpeed;
        NPCUnit movableUnit = controller.Unit as NPCUnit;
        if(movableUnit == null) {
            return null;
        }
        ActiveMoveState moveState = new ActiveMoveState(controller.MapSpaceTarget, movableUnit.MoveController, speed);
        return moveState;
    }
}

public class ActiveMoveState : ActiveAIState {

    private IntVector3 _nextDestination;
    private NPCMoveController _moveController;

    public ActiveMoveState(IntVector3 nextDestination, MoveController moveController, float speed) {
        // Get path to next destination here
        _moveController = moveController as NPCMoveController;
        if(_moveController == null) {
            CustomLogger.Error(nameof(ActiveMoveState), $"Move controller was not of type [{nameof(NPCMoveController)}]");
            return;
        }
        _moveController.OnArrivedTargetDestination += OnArrivedTargetDestination;
        _moveController.SetDestination(speed, nextDestination);
    }

    private void OnArrivedTargetDestination() {
        _moveController.OnArrivedTargetDestination -= OnArrivedTargetDestination;
        SetNextTransition(AIStateTransitionId.OnUnitMoveComplete);
    }
}