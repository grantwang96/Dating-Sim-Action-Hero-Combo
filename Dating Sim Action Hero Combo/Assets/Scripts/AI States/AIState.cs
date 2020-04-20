using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class AIStateDataObject : ScriptableObject {

    [SerializeField] protected List<AIStateDataObject> _subStates = new List<AIStateDataObject>();

    public virtual ActiveAIState Enter(NPCUnitController controller, AIStateInitializationData initData = null) {
        ActiveAIState newState = GenerateActiveAIState(controller);
        for (int i = 0; i < _subStates.Count; i++) {
            ActiveAIState subState = _subStates[i].Enter(controller);
            newState.AddAISubState(subState);
        }
        return newState;
    }
    
    // start state and initialize some active AI State Data
    public virtual bool Execute(ActiveAIState activeAIState) {
        bool cancelAll = activeAIState.OnExecute();
        for (int i = 0; i < activeAIState.SubStates.Count; i++) {
            ActiveAIState subData = activeAIState.SubStates[i];
            if (_subStates[i].Execute(subData)) {
                cancelAll = true;
                break;
            }
        }
        return cancelAll;
    }

    public virtual void Exit(ActiveAIState activeAIState) {
        activeAIState.OnExit();
    } // exit state behaviour
    
    protected abstract ActiveAIState GenerateActiveAIState(NPCUnitController controller);
}

[System.Serializable]
public class AIStateTransitionEntry {

    [SerializeField] private AIStateTransitionId _transitionId;
    [SerializeField] private List<AIStateDataObject> _states = new List<AIStateDataObject>();

    public AIStateTransitionId TransitionId => _transitionId;
    public List<AIStateDataObject> States => _states;
}

public enum AIStateTransitionId {
    OnUnitInitialized,
    OnUnitReadyToMove,
    OnUnitMoveComplete,
    OnUnitEnemyDiscovered,
    OnUnitAlerted,
    OnUnitEnemyLost,
    OnUnitEnemyRefound,
    OnNextTargetSet,
    OnUnitDefeated,
    OnUnitReadyDespawn,
    OnUnitAllyAlert,
    OnUnitMeleeAttack,
    OnUnitRangedAttack,
    OnUnitAllyLostEnemy,
    OnUnitTakeDamage,
    OnUnitChase,
    OnUnitReadyToQuickMove,
    OnCombatNoiseHeard
}

// information about the current AI State
public class ActiveAIState {

    protected List<ActiveAIState> _subStates = new List<ActiveAIState>();
    public IReadOnlyList<ActiveAIState> SubStates => _subStates;

    public event Action<AIStateTransitionId> OnReadyToTransition;
    
    public void AddAISubState(ActiveAIState data) {
        _subStates.Add(data);
        data.OnReadyToTransition += SetNextTransition;
    }

    protected void SetNextTransition(AIStateTransitionId transitionId) {
        OnReadyToTransition?.Invoke(transitionId);
    }

    public virtual bool OnExecute() {
        return false;
    }

    public virtual void OnExit() {
        for (int i = 0; i < SubStates.Count; i++) {
            SubStates[i].OnReadyToTransition -= SetNextTransition;
        }
    }
}

// any additional data that the AI State needs will live here
public class AIStateInitializationData {

}
