using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class AIState : MonoBehaviour {

    [SerializeField] protected bool _active;
    [SerializeField] protected NPCUnit _unit;
    [SerializeField] protected UnitAIStateMachine _stateMachine;

    protected bool _initialized;

    public AIState ParentState { get; protected set; }
    public string StateId { get; protected set; }
    public event Action<AIState, AIStateInitializationData> OnReadyToTransitionState;

    private void Awake() {
        ParentState = transform.GetComponent<AIState>();
    }

    private void Start() {
        InitializeId();
    }

    public void InitializeId() {
        if (_initialized) {
            return;
        }
        StateId = name;
        if (ParentState != null) {
            ParentState.InitializeId();
            StateId = $"{ParentState.StateId}/{StateId}";
        }
        _initialized = true;
    }

    public virtual void Enter(AIStateInitializationData initData = null) {
        if(ParentState != null) {
            ParentState.Enter(initData);
            ParentState.OnReadyToTransitionState += SetReadyToTransition;
        }
        _active = true;
    }
    
    // start state and initialize some active AI State Data
    public virtual void Execute() {
        if(ParentState != null) {
            ParentState.Execute();
        }
    }

    // exit state behaviour
    public virtual void Exit() {
        if(ParentState != null) {
            ParentState.Exit();
            ParentState.OnReadyToTransitionState -= SetReadyToTransition;
        }
        _active = false;
    }

    protected void SetReadyToTransition(AIState state, AIStateInitializationData initData = null) {
        OnReadyToTransitionState?.Invoke(state, initData);
    }
}

// any additional data that the AI State needs will live here
public class AIStateInitializationData {

}
