using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultEnemyManagerState : EnemyManagerState {

    public override void OnControllerReadyToTransition(AIStateTransitionId transitionId, IEnemyController controller) {
        base.OnControllerReadyToTransition(transitionId, controller);
        switch (transitionId) {
            case AIStateTransitionId.OnUnitIdleFinished:
                AssignNewMoveTarget(controller);
                break;
            case AIStateTransitionId.OnUnitEnemyLost:
                // change manager state (next state will set controller states on Enter)
                return;
            case AIStateTransitionId.OnUnitDefeated:
                OnEnemyDefeated(controller);
                // change manager state probably
                break;
        }
        // probably continue normally
        controller.TransitionState(transitionId);
    }

    protected override void OnEnemyDefeated(IEnemyController controller) {
        base.OnEnemyDefeated(controller);
    }

    private void AssignNewMoveTarget(IEnemyController controller) {
        int searchRadius = Random.Range(controller.Data.WanderRadiusMin, controller.Data.WanderRadiusMax);
        List<IntVector3> traversableTiles = MapService.GetTraversableTiles(searchRadius, controller.MapPosition, controller.Data.WanderRadiusMin);
        IntVector3 nextDestination = traversableTiles[Random.Range(0, traversableTiles.Count)];
        controller.MapSpaceTarget = nextDestination;
    }
}
