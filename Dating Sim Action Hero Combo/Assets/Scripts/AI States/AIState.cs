using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class AIState : MonoBehaviour {

    [SerializeField] protected NPCUnit _unit;
    [SerializeField] protected List<AIState> _subStates = new List<AIState>();
    [SerializeField] protected List<AIStateTransitionEntry> _aiStateTransitionsEntries = new List<AIStateTransitionEntry>();

    protected NPCUnitController _controller;

    public event Action<AIStateTransitionId> OnReadyToTransition;

    private void Awake() {
        _unit.OnUnitInitialized += OnUnitInitialized;
    }

    protected virtual void OnUnitInitialized(NPCUnitController controller) {
        _controller = controller;
    }

    public virtual void Enter(AIStateInitializationData initData = null) {
        Debug.Log($"Entering state {name}");
        for (int i = 0; i < _subStates.Count; i++) {
            _subStates[i].Enter();
            _subStates[i].OnReadyToTransition += OnSubStateReadyToTransition;
        }
    }
    
    // start state and initialize some active AI State Data
    public virtual bool Execute() {
        bool cancelAll = false;
        for (int i = 0; i < _subStates.Count; i++) {
            _subStates[i].Execute();
            if (_subStates[i].Execute()) {
                cancelAll = true;
                break;
            }
        }
        return cancelAll;
    }

    // exit state behaviour
    public virtual void Exit() {
        for (int i = 0; i < _subStates.Count; i++) {
            _subStates[i].Exit();
            _subStates[i].OnReadyToTransition -= OnSubStateReadyToTransition;
        }
    }

    public virtual void GetStatesFor(AIStateTransitionId transitionId, List<AIState> possibleStates) {
        for(int i = 0; i < _aiStateTransitionsEntries.Count; i++) {
            if(_aiStateTransitionsEntries[i].TransitionId == transitionId) {
                possibleStates.AddRange(_aiStateTransitionsEntries[i].States);
            }
        }
        for(int i = 0; i < _subStates.Count; i++) {
            _subStates[i].GetStatesFor(transitionId, possibleStates);
        }
    }

    protected void SetNextTransition(AIStateTransitionId transitionId) {
        OnReadyToTransition?.Invoke(transitionId);
    }

    protected void OnSubStateReadyToTransition(AIStateTransitionId transitionId) {
        SetNextTransition(transitionId);
    }
}

[System.Serializable]
public class AIStateTransitionEntry {

    [SerializeField] private AIStateTransitionId _transitionId;
    [SerializeField] private List<AIState> _states = new List<AIState>();

    public AIStateTransitionId TransitionId => _transitionId;
    public List<AIState> States => _states;
}

public enum AIStateTransitionId {
    OnUnitInitialized,
    OnIdleFinished,
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

// any additional data that the AI State needs will live here
public class AIStateInitializationData {

}
