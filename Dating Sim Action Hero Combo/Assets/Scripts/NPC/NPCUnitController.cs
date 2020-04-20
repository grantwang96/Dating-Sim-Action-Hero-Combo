using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NPCUnitController : UnitController
{
    private AIStateDataObject _currentState;
    private ActiveAIState _activeAIStateData; // information about the current AI State
    
    public AIStateDataObject QueuedState { get; set; }
    public Unit FocusedTarget { get; set; }

    public event Action<AIStateTransitionId, NPCUnitController> OnAIStateReadyToTransition;

    protected override void OnTakeDamage(int damage, DamageType damageType, Unit attacker) {
        base.OnTakeDamage(damage, damageType, attacker);
        if (Health <= 0) {
            CurrentAIStateReadyToTransition(AIStateTransitionId.OnUnitDefeated);
        }
        // change state ???
    }

    protected virtual void CurrentAIStateReadyToTransition(AIStateTransitionId transitionId) {
        _activeAIStateData.OnReadyToTransition -= CurrentAIStateReadyToTransition;
        OnAIStateReadyToTransition?.Invoke(transitionId, this);
    }
    
    // can be called internally or used as override by manager
    public virtual void TransitionState(AIStateTransitionId transitionId) {
        if (_currentState != null) {
            _currentState.Exit(_activeAIStateData);
        }
        List<AIStateDataObject> possibleNextStates = Data.GetStateForTransitionId(transitionId);
        if (possibleNextStates.Count == 0) {
            CustomLogger.Error(nameof(UnitController), $"Unit Data Object {Data.name} does not have any states for transition id {transitionId}");
            _currentState = null;
            return;
        }
        _currentState = possibleNextStates[UnityEngine.Random.Range(0, possibleNextStates.Count)];
        Debug.Log($"Transitioning to state {_currentState.name}");
        _activeAIStateData = _currentState.Enter(this);
        _activeAIStateData.OnReadyToTransition += CurrentAIStateReadyToTransition;
    }
    
    // called by Unit manager on update loop
    public virtual void ExecuteState() {
        if (_currentState == null) {
            return;
        }
        _currentState.Execute(_activeAIStateData);
    }
}
