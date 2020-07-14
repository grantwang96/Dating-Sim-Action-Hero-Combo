using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAIStateMachine : MonoBehaviour
{
    [SerializeField] private NPCUnit _unit;
    [SerializeField] private AIState _startingState;
    [SerializeField] private UnitMessageAIStateOverride[] _unitMessageAIStateOverrides;

    private AIState _currentState;
    private NPCMessageInterpreter _messageInterpeter;
    private Dictionary<UnitMessage, AIState> _messageOverrideStates = new Dictionary<UnitMessage, AIState>();
    private Queue<AIStateDataEntry> _queuedStateList = new Queue<AIStateDataEntry>();

    private void Awake() {
        _unit.OnUnitInitialized += OnUnitInitialized;
        _messageInterpeter = new NPCMessageInterpreter(_unit);
        OnUnitInitialized(); // initialize may run before Awake() is called???
        InitializeMessageAIStateOverrides();
    }

    private void InitializeMessageAIStateOverrides() {
        _messageOverrideStates.Clear();
        for(int i = 0; i < _unitMessageAIStateOverrides.Length; i++) {
            _messageOverrideStates.Add(_unitMessageAIStateOverrides[i].UnitMessage, _unitMessageAIStateOverrides[i].AIState);
        }
    }

    // Start is called before the first frame update
    private void Start() {
        OnReadyToTransitionState(_startingState);
    }
    
    private void Update() {
        // execute the current AI State
        if(_currentState != null) {
            _currentState.Execute();
        }
    }

    private void OnUnitInitialized() {
        _unit.OnUnitDefeated += OnUnitDefeated;
        _unit.OnUnitMessageReceived += OnAllianceMessageReceived;
    }

    private void OnUnitDefeated(Unit unit) {
        _unit.OnUnitDefeated -= OnUnitDefeated;
        _unit.OnUnitMessageReceived += OnAllianceMessageReceived;
    }

    private void OnAllianceMessageReceived(NPCUnit ally, UnitMessage message) {
        // don't override state if this was the sender
        if(ally == _unit) {
            return;
        }
        // retrieve the appropriate state
        if(!_messageOverrideStates.TryGetValue(message, out AIState nextState)) {
            return;
        }
        _messageInterpeter.InterpetMessage(ally, message);
        OnReadyToTransitionState(nextState);
    }

    // listener that changes AI State
    private void OnReadyToTransitionState(AIState nextState, AIStateInitializationData initData = null) {
        // exit the current state
        if(_currentState != null) {
            _currentState.Exit(nextState);
            _currentState.OnReadyToTransitionState -= OnReadyToTransitionState;
        }
        // if there is a state queue override, continue the queue instead
        if(_queuedStateList.Count != 0) {
            AIStateDataEntry entry = _queuedStateList.Dequeue();
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

public class AIStateDataEntry {
    public AIState State;
    public AIStateInitializationData Data;
}

[System.Serializable]
public class UnitMessageAIStateOverride {
    [SerializeField] private UnitMessage _unitMessage;
    [SerializeField] private AIState _aiState;

    public UnitMessage UnitMessage => _unitMessage;
    public AIState AIState => _aiState;
}
