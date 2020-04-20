﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGuardEnemyManagerState : EnemyManagerState
{
    public override void OnControllerReadyToTransition(AIStateTransitionId transitionId, NPCUnitController controller) {
        base.OnControllerReadyToTransition(transitionId, controller);
        switch (transitionId) {
            case AIStateTransitionId.OnUnitReadyToMove:
                AssignNewWanderTarget(controller);
                break;
            case AIStateTransitionId.OnUnitAlerted:
                // enter an aggro state?
                BroadcastUnitStateChange(AIStateTransitionId.OnUnitAllyAlert, controller);
                break;
            case AIStateTransitionId.OnUnitEnemyLost:
                MoveGuardsToLastKnownLoc(controller);
                break;
            case AIStateTransitionId.OnUnitDefeated:
                OnEnemyDefeated(controller);
                // change manager state probably
                break;
        }
        // probably continue normally
        controller.TransitionState(transitionId);
    }

    private void MoveGuardsToLastKnownLoc(NPCUnitController controller) {
        IntVector3 targetPosition = controller.FocusedTarget.MoveController.MapPosition;
        for (int i = 0; i < EnemyManager.Instance.AllEnemies.Count; i++) {
            // TODO: filter by enemy type
            EnemyController enemy = EnemyManager.Instance.AllEnemies[i];
            if(enemy == controller) {
                continue;
            }
            enemy.MapSpaceTarget = targetPosition;
            enemy.TransitionState(AIStateTransitionId.OnUnitAllyLostEnemy);
        }
        AssignRunTarget(controller, targetPosition);
    }

    private void AssignRunTarget(NPCUnitController controller, IntVector3 target) {
        controller.MapSpaceTarget = target;
    }

    private void AssignNewWanderTarget(NPCUnitController controller) {
        int searchRadius = Random.Range(controller.Data.WanderRadiusMin, controller.Data.WanderRadiusMax);
        List<IntVector3> traversableTiles = MapService.GetTraversableTiles(searchRadius, controller.MapPosition, controller.Data.WanderRadiusMin);
        IntVector3 nextDestination = traversableTiles[Random.Range(0, traversableTiles.Count)];
        controller.MapSpaceTarget = nextDestination;
    }
}
