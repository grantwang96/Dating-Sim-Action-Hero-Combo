using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI State/Move")]
public class AIState_Move : AIStateDataObject
{
    [SerializeField] private bool _fullSpeed;

    protected override ActiveAIState GenerateActiveAIState(EnemyController enemyController) {
        float speed = _fullSpeed ? enemyController.Data.RunSpeed : enemyController.Data.WalkSpeed;
        ActiveMoveState moveState = new ActiveMoveState(enemyController.MapSpaceTarget, enemyController.Unit.MoveController, speed);
        return moveState;
    }
}

public class ActiveMoveState : ActiveAIState {

    private IntVector3 _nextDestination;
    private NPCMoveController _moveController;

    public ActiveMoveState(IntVector3 nextDestination, NPCMoveController moveController, float speed) {
        // Get path to next destination here
        _moveController = moveController;
        moveController.OnArrivedTargetDestination += OnArrivedTargetDestination;
        moveController.SetDestination(speed, nextDestination);
    }

    private void OnArrivedTargetDestination() {
        _moveController.OnArrivedTargetDestination -= OnArrivedTargetDestination;
        SetNextTransition(AIStateTransitionId.OnUnitWanderFinished);
    }
}