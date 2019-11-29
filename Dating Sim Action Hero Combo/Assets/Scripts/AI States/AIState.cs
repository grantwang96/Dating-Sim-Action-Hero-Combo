using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class AIStateDataObject : ScriptableObject {

    [SerializeField] protected List<AIStateDataObject> _subStates = new List<AIStateDataObject>();

    public virtual ActiveAIState Enter(EnemyController enemyController, AIStateInitializationData initData = null) {
        ActiveAIState newState = GenerateActiveAIState(enemyController);
        for (int i = 0; i < _subStates.Count; i++) {
            ActiveAIState subState = _subStates[i].Enter(enemyController);
            newState.AddAISubState(subState);
        }
        return newState;
    }
    
    // start state and initialize some active AI State Data
    public virtual void Execute(ActiveAIState activeAIState) {
        activeAIState.OnExecute();
        for (int i = 0; i < _subStates.Count; i++) {
            ActiveAIState subData = activeAIState.SubStates[i];
            _subStates[i].Execute(subData);
        }
    }

    public virtual void Exit() { } // exit state behaviour
    
    protected abstract ActiveAIState GenerateActiveAIState(EnemyController enemyController);
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
    OnUnitIdleFinished,
    OnUnitWanderFinished,
    OnUnitEnemySeen,
    OnUnitEnemyLost,
    OnNextTargetSet,
}

// information about the current AI State
public class ActiveAIState {

    protected List<ActiveAIState> _subStates = new List<ActiveAIState>();
    public IReadOnlyList<ActiveAIState> SubStates => _subStates;

    public event Action<AIStateTransitionId> OnReadyToTransition;
    
    ~ActiveAIState() {
        for(int i = 0; i < SubStates.Count; i++) {
            SubStates[i].OnReadyToTransition -= SetNextTransition;
        }
    }

    public void AddAISubState(ActiveAIState data) {
        _subStates.Add(data);
        data.OnReadyToTransition += SetNextTransition;
    }

    protected void SetNextTransition(AIStateTransitionId transitionId) {
        OnReadyToTransition?.Invoke(transitionId);
    }

    public virtual void OnExecute() {
        
    }
}

// any additional data that the AI State needs will live here
public class AIStateInitializationData {

}
