﻿using System.Collections;
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
    public bool Active => _active;

    public event Action<AIState, AIStateInitializationData> OnReadyToTransitionState;

    private void Awake() {
        ParentState = transform.parent?.GetComponent<AIState>();
    }

    private void Start() {
        InitializeId();
    }

    // initialize StateId and Parent's StateId
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

    // check if given state is on this state's path
    public bool IsOnMyPath(AIState state) {
        if(ParentState == null) {
            return false;
        }
        if (ParentState == state) {
            return true;
        }
        return ParentState.IsOnMyPath(state);
    }

    // enter state behaviour
    public virtual void Enter(AIStateInitializationData initData = null) {
        // if has parent and parent is not active, enter parent state
        if(ParentState != null && !ParentState.Active) {
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
    public virtual void Exit(AIState nextState) {
        // if this state is on the next state's path, ignore
        if (nextState.IsOnMyPath(this)) {
            return;
        }
        // Exit the parent state, if available
        if(ParentState != null) {
            ParentState.Exit(nextState);
            ParentState.OnReadyToTransitionState -= SetReadyToTransition;
        }
        _active = false;
    }

    protected void SetReadyToTransition(AIState state, AIStateInitializationData initData = null) {
        OnReadyToTransitionState?.Invoke(state, initData);
    }
}

// any additional data that the AI State needs will live here
[System.Serializable]
public class AIStateInitializationData {

}
