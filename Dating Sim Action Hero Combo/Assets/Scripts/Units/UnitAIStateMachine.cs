using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAIStateMachine : MonoBehaviour
{
    [SerializeField] private Unit _unit;
    [SerializeField] private AIState _startingState;

    private AIState _currentState;

    private Queue<QueuedAIStateEntry> _queuedStateList = new Queue<QueuedAIStateEntry>();

    // Start is called before the first frame update
    private void Start() {
        OnReadyToTransitionState(_startingState);
    }
    
    private void Update() {
        if(_currentState != null) {
            _currentState.Execute();
        }
    }

    private void OnReadyToTransitionState(AIState nextState, AIStateInitializationData initData = null) {
        if(_currentState != null) {
            _currentState.Exit(nextState);
            _currentState.OnReadyToTransitionState -= OnReadyToTransitionState;
        }
        if(_queuedStateList.Count != 0) {
            QueuedAIStateEntry entry = _queuedStateList.Dequeue();
            SetNextState(entry.State, entry.Data);
            return;
        }
        SetNextState(nextState, initData);
    }

    private void SetNextState(AIState state, AIStateInitializationData initData = null) {
        _currentState = state;
        _currentState.OnReadyToTransitionState += OnReadyToTransitionState;
        _currentState.Enter(initData);
    }
}

public class QueuedAIStateEntry {
    public AIState State;
    public AIStateInitializationData Data;
}
