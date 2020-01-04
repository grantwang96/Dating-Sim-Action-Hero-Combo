using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IEnemyController : IUnitController {
    float Speed { get; }
}

public class EnemyController : IEnemyController
{
    public int Health { get; private set; }
    public float Speed { get; private set; }
    public Unit Unit { get; private set; }
    public UnitData Data { get; private set; }
    public WeaponSlot EquippedWeapon { get; private set; }

    public IntVector3 MapPosition => Unit.MapPosition;
    private IntVector3 _mapSpaceTarget;
    public IntVector3 MapSpaceTarget {
        get { return _mapSpaceTarget; }
        set {
            _mapSpaceTarget = value;
        }
    }
    public Unit FocusedTarget { get; set; }

    public event Action<AIStateTransitionId, IUnitController> OnAIStateReadyToTransition;
    public event Action<IUnitController> OnUnitDefeated;

    private AIStateDataObject _currentState;
    private ActiveAIState _activeAIStateData; // information about the current AI State

    public EnemyController(EnemyData enemyData, EnemyUnit unit) {
        Data = enemyData;
        Health = enemyData.MaxHealth;
        Speed = enemyData.WalkSpeed;
        EquippedWeapon = new WeaponSlot(enemyData.EquippedWeapon);

        Unit = unit;
        unit.Initialize(enemyData.AnimatorController, .5f); // temp
        Unit.SetUnitTags(UnitTags.Enemy);
        UnitsManager.Instance.RegisterUnit(Unit);
        unit.Spawn();

        // subscribe to events?
        SubscribeToEvents();
    }

    public void Dispose() {
        UnsubscribeToEvents();
        UnitsManager.Instance.DeregisterUnit(Unit);
        EnemyUnit enemyUnit = Unit as EnemyUnit;
        if (enemyUnit != null) {
            enemyUnit.Despawn();
        }
    }

    private void SubscribeToEvents() {
        Unit.OnTakeDamage += OnTakeDamage;
        Unit.OnHealDamage += OnHealDamage;
        Unit.OnCombatSoundHeard += OnSoundHeard;

        EnemyManager.Instance.OnUnitBroadcastMessage += OnUnitBroadcastMessage;
    }

    private void UnsubscribeToEvents() {
        Unit.OnTakeDamage -= OnTakeDamage;
        Unit.OnHealDamage -= OnHealDamage;
        Unit.OnCombatSoundHeard -= OnSoundHeard;

        EnemyManager.Instance.OnUnitBroadcastMessage -= OnUnitBroadcastMessage;
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

    private void OnUnitBroadcastMessage(AIStateTransitionId transitionId, IUnitController controller) {
        if(controller == this) {
            return;
        }
        TransitionState(transitionId);
    }

    // can be called internally or used as override by manager
    public void TransitionState(AIStateTransitionId transitionId) {
        List<AIStateDataObject> possibleNextStates = Data.GetStateForTransitionId(transitionId);
        if(possibleNextStates.Count == 0) {
            CustomLogger.Error(nameof(EnemyController), $"Enemy Data Object {Data.name} does not have any states for transition id {transitionId}");
            _currentState = null;
            return;
        }
        // don't attempt to re-enter current state
        if (possibleNextStates.Contains(_currentState)) {
            return;
        }
        if (_currentState != null) {
            _currentState.Exit(_activeAIStateData);
        }
        _currentState = possibleNextStates[UnityEngine.Random.Range(0, possibleNextStates.Count)];
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
        if(Health <= 0) {
            OnCurrentStateReadyToTransition(AIStateTransitionId.OnUnitDefeated);
            EnemyManager.Instance.DespawnEnemy(this);
            return;
        }
        if (attacker.UnitTags.HasFlag(UnitTags.Player) || attacker.UnitTags.HasFlag(UnitTags.Law_Enforcement)) {
            FocusedTarget = attacker;
        }
        OnCurrentStateReadyToTransition(AIStateTransitionId.OnUnitTakeDamage);
    }

    private void OnHealDamage(int damage) {
        Health += damage;
    }

    private void OnSoundHeard(IntVector3 origin, Unit source) {
        // determine if we need to change state for this
        // don't react to sounds from friendlies
        if (source != null && (source == Unit || source.UnitTags.HasFlag(Unit.UnitTags))) {
            return;
        }
        MapSpaceTarget = origin;
        OnCurrentStateReadyToTransition(AIStateTransitionId.OnUnitReadyToQuickMove);
    }

    #endregion
}
