using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NPCUnitController : UnitController
{
    protected AIState _currentState;

    protected NPCUnit _unit;
    public override Unit Unit => _unit;
    public AIState QueuedState { get; set; }
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
        OnAIStateReadyToTransition?.Invoke(transitionId, this);
    }
    
    // can be called internally or used as override by manager
    public virtual void TransitionState(AIStateTransitionId transitionId) {
        if (_currentState != null) {
            List<AIState> possibleNextStates = new List<AIState>();
            _currentState.GetStatesFor(transitionId, possibleNextStates);
            // if there's nothing to transition to, exit
            if (possibleNextStates.Count == 0) {
                CustomLogger.Log(nameof(NPCUnitController), $"No transitions for id {transitionId}");
                return;
            }
            _currentState.OnReadyToTransition -= CurrentAIStateReadyToTransition;
            _currentState.Exit();
            _currentState = possibleNextStates[UnityEngine.Random.Range(0, possibleNextStates.Count)];
        } else {
            _currentState = _unit.OnUnitInitializedState;
        }
        _currentState.Enter();
        _currentState.OnReadyToTransition += CurrentAIStateReadyToTransition;
    }
    
    // called by Unit manager on update loop
    public virtual void ExecuteState() {
        if (_currentState == null) {
            return;
        }
        _currentState.Execute();
    }
}
