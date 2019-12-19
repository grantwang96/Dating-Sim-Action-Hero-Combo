using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultEnemyManagerState : EnemyManagerState {

    public override void OnControllerReadyToTransition(AIStateTransitionId transitionId, IUnitController controller) {
        base.OnControllerReadyToTransition(transitionId, controller);
        switch (transitionId) {
            case AIStateTransitionId.OnUnitIdleFinished:
                AssignNewWanderTarget(controller);
                break;
            case AIStateTransitionId.OnUnitAlerted:
                // enter an aggro state?
                MoveGuardsToLastKnownLoc(controller);
                BroadcastUnitStateChange(AIStateTransitionId.OnUnitAllyAlert, controller);
                break;
            case AIStateTransitionId.OnUnitEnemyLost:
                // potentially change manager state
                // TODO: depending on enemy type, do something different (some might chase, some might hold position)
                IntVector3 targetPosition = controller.FocusedTarget.MapPosition;
                AssignRunTarget(controller, targetPosition);
                break;
            case AIStateTransitionId.OnUnitDefeated:
                OnEnemyDefeated(controller);
                // change manager state probably
                break;
        }
        // probably continue normally
        controller.TransitionState(transitionId);
    }

    private void MoveGuardsToLastKnownLoc(IUnitController controller) {
        IntVector3 targetPosition = controller.FocusedTarget.MapPosition;

        // TODO: filter by enemy type

        List<IntVector3> _availableSpots = MapService.GetTraversableTiles(EnemyManager.Instance.AllEnemies.Count, targetPosition, 1);
        for (int i = 0; i < EnemyManager.Instance.AllEnemies.Count; i++) {
            IEnemyController enemy = EnemyManager.Instance.AllEnemies[i];
            if (enemy == controller) {
                continue;
            }
            enemy.MapSpaceTarget = targetPosition;
            if(_availableSpots.Count > 0) {
                enemy.MapSpaceTarget = _availableSpots[0];
                _availableSpots.RemoveAt(0);
            }
            // enemy.TransitionState(AIStateTransitionId.OnUnitAllyLostEnemy);
        }
        AssignRunTarget(controller, targetPosition);
    }

    protected override void OnEnemyDefeated(IUnitController controller) {
        base.OnEnemyDefeated(controller);
    }

    private void AssignRunTarget(IUnitController controller, IntVector3 target) {
        controller.MapSpaceTarget = target;
    }

    private void AssignNewWanderTarget(IUnitController controller) {
        int searchRadius = Random.Range(controller.Data.WanderRadiusMin, controller.Data.WanderRadiusMax);
        List<IntVector3> traversableTiles = MapService.GetTraversableTiles(searchRadius, controller.MapPosition, controller.Data.WanderRadiusMin);
        IntVector3 nextDestination = traversableTiles[Random.Range(0, traversableTiles.Count)];
        controller.MapSpaceTarget = nextDestination;
    }
}
