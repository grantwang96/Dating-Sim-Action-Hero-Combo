using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// State for the overall enemy manager
/// </summary>
public abstract class EnemyManagerState : MonoBehaviour {

    public event Action<EM_StateTransitionId> NextStateUpdated;
    public event Action<AIStateTransitionId, NPCUnitController> OnBroadcastUnitStateChange;

    // what should happen at the start of this state
    public virtual void Enter() {

    }
    
    // when an enemy controller reports a change of state, what should happen?
    public virtual void OnControllerReadyToTransition(AIStateTransitionId transitionId, NPCUnitController controller) {

    }

    public virtual void Exit() {

    }

    protected virtual void OnEnemyDefeated(NPCUnitController controller) {

    }

    protected void BroadcastManagerStateChange(EM_StateTransitionId transitionId) {
        NextStateUpdated?.Invoke(transitionId);
    }

    protected void BroadcastUnitStateChange(AIStateTransitionId transitionId, NPCUnitController controller) {
        OnBroadcastUnitStateChange?.Invoke(transitionId, controller);
    }
}

public enum EM_StateTransitionId {
    OnGameStart,
    OnPlayerSpotted,
    OnPlayerLost,
    OnEnemyDefeated
}