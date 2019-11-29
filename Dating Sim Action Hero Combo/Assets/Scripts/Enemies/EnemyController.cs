using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IEnemyController {
    int Health { get; }
    float Speed { get; }
    EnemyUnit Unit { get; }
    EnemyData Data { get; }

    IntVector3 MapPosition { get; }
    IntVector3 MapSpaceTarget { get; set; }

    event Action<AIStateTransitionId, IEnemyController> OnAIStateReadyToTransition;

    void TransitionState(AIStateTransitionId transitionId);
    void ExecuteState();
}

public class EnemyController : IEnemyController
{
    public int Health { get; private set; }
    public float Speed { get; private set; }
    public EnemyUnit Unit { get; private set; }
    public EnemyData Data { get; private set; }

    public IntVector3 MapPosition => Unit.MoveController.MapPosition;
    public IntVector3 MapSpaceTarget { get; set; }

    public event Action<AIStateTransitionId, IEnemyController> OnAIStateReadyToTransition;

    private AIStateDataObject _currentState;
    private ActiveAIState _activeAIStateData; // information about the current AI State

    public EnemyController(EnemyData enemyData, EnemyUnit unit) {
        Data = enemyData;
        Health = enemyData.MaxHealth;
        Speed = enemyData.WalkSpeed;
        Unit = unit;
        Unit.Initialize(enemyData.AnimatorController, .5f); // temp
        
        // subscribe to events?
    }

    // when this object gets GCed
    ~EnemyController() {
        // unsubscribe to events
    }

    // called by EnemyManager on update loop
    public void ExecuteState() {
        if(_currentState == null) {
            return;
        }
        _currentState.Execute(_activeAIStateData);
    }

    // listens for update from active AI state data
    private void OnCurrentStateReadyToTransition(AIStateTransitionId transitionId) {
        _activeAIStateData.OnReadyToTransition -= OnCurrentStateReadyToTransition;
        OnAIStateReadyToTransition?.Invoke(transitionId, this);
    }

    // can be called internally or used as override by manager
    public void TransitionState(AIStateTransitionId transitionId) {
        if(_currentState != null) {
            _currentState.Exit();
        }
        List<AIStateDataObject> possibleNextStates = Data.GetStateForTransitionId(transitionId);
        _currentState = possibleNextStates[UnityEngine.Random.Range(0, possibleNextStates.Count)];
        _activeAIStateData = _currentState.Enter(this);
        _activeAIStateData.OnReadyToTransition += OnCurrentStateReadyToTransition;
    }
}
