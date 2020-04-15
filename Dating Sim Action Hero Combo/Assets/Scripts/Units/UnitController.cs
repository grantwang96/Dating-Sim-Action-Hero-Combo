using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IUnitController {
    int Health { get; }
    Unit Unit { get; }
    UnitData Data { get; }

    IntVector3 MapPosition { get; }
    IntVector3 MapSpaceTarget { get; set; }
    Unit FocusedTarget { get; set; }
    Weapon EquippedWeapon { get; }

    event Action<AIStateTransitionId, IUnitController> OnAIStateReadyToTransition;
    event Action<IUnitController> OnUnitDefeated;

    void TransitionState(AIStateTransitionId transitionId);
    void ExecuteState();
    void Dispose();
}

public class UnitController : IUnitController
{
    public int Health { get; private set; }
    public Unit Unit { get; private set; }
    public UnitData Data { get; private set; }

    public Weapon EquippedWeapon { get; }
    public IntVector3 MapPosition => Unit.MoveController.MapPosition;
    private IntVector3 _mapSpaceTarget;
    public IntVector3 MapSpaceTarget {
        get { return _mapSpaceTarget; }
        set {
            _mapSpaceTarget = value;
        }
    }
    public Unit FocusedTarget { get; set; }
    public AIStateDataObject QueuedState { get; set; }

    public event Action<AIStateTransitionId, IUnitController> OnAIStateReadyToTransition;
    public event Action<IUnitController> OnUnitDefeated;

    private AIStateDataObject _currentState;
    private ActiveAIState _activeAIStateData; // information about the current AI State

    public UnitController() {

    }

    public void Dispose() {
        UnsubscribeToEvents();
    }

    private void SubscribeToEvents() {
        Unit.OnTakeDamage += OnTakeDamage;
        Unit.OnHealDamage += OnHealDamage;
    }

    private void UnsubscribeToEvents() {
        Unit.OnTakeDamage -= OnTakeDamage;
        Unit.OnHealDamage -= OnHealDamage;
    }

    // called by Unit manager on update loop
    public void ExecuteState() {
        if (_currentState == null) {
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
        _activeAIStateData.OnReadyToTransition += OnCurrentStateReadyToTransition;
    }

    #region LISTENERS

    private void OnTakeDamage(int damage, DamageType damageType, Unit attacker) {
        int totalDamage = damage;
        // if this unit is resistant to this damage
        if ((Data.Resistances & damageType) != 0) {
            totalDamage /= 4;
            totalDamage = Mathf.Max(totalDamage, 1);
        }

        // update health and states
        Health -= totalDamage;
        if (Health <= 0) {
            OnCurrentStateReadyToTransition(AIStateTransitionId.OnUnitDefeated);
            UnitsManager.Instance.DeregisterUnit(Unit);
        }
        // change state ???
    }

    private void OnHealDamage(int damage) {
        Health += damage;
    }

    #endregion
}
