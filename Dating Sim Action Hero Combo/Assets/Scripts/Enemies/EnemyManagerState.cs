using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State for the overall enemy manager
/// </summary>
public abstract class EnemyManagerState
{
    // what should happen at the start of this state
    public virtual void Enter() {

    }

    // when an enemy controller reports a change of state, what should happen?
    public virtual void OnReadyToTransition(AIStateTransitionId transitionId, IEnemyController controller) {
        
    }

    public virtual void Exit() {

    }
}

public class DefaultEnemyManagerState : EnemyManagerState {

    public override void OnReadyToTransition(AIStateTransitionId transitionId, IEnemyController controller) {
        base.OnReadyToTransition(transitionId, controller);
        switch (transitionId) {
            case AIStateTransitionId.OnUnitIdleFinished:
                AssignNewWanderTarget(controller);
                break;
            case AIStateTransitionId.OnUnitEnemyLost:
                // change manager state (next state will set controller states on Enter)
                break;
        }
        // probably continue normally
        controller.TransitionState(transitionId);
    }

    private void AssignNewWanderTarget(IEnemyController controller) {
        int searchRadius = Random.Range(controller.Data.WanderRadiusMin, controller.Data.WanderRadiusMax);
        List<IntVector3> traversableTiles = MapService.GetTraversableTiles(searchRadius, controller.MapPosition, controller.Data.WanderRadiusMin);
        IntVector3 nextDestination = traversableTiles[Random.Range(0, traversableTiles.Count)];
        controller.MapSpaceTarget = nextDestination;
    }
}
