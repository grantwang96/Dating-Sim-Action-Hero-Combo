using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyController : NPCUnitController
{
    private const int EnemyIdLength = 5;
    public float Speed { get; private set; }

    public EnemyController(EnemyData enemyData, EnemyUnit unit, string overrideId = "") {

        if (string.IsNullOrEmpty(overrideId)) {
            UnitId = $"Enemy_{StringGenerator.RandomString(EnemyIdLength)}";
        } else {
            UnitId = overrideId;
        }

        Data = enemyData;
        Health = enemyData.MaxHealth;
        Speed = enemyData.WalkSpeed;
        EquippedWeapon = new Weapon(enemyData.EquippedWeapon);

        _unit = unit;
        unit.Initialize(this, enemyData.AnimatorController, .5f); // temp
        Unit.SetUnitTags(UnitTags.Enemy);
        UnitsManager.Instance.RegisterUnit(Unit);
        unit.Spawn();

        // subscribe to events?
        SubscribeToEvents();
    }

    public override void Dispose() {
        UnsubscribeToEvents();
        UnitsManager.Instance.DeregisterUnit(Unit);
        EnemyUnit enemyUnit = Unit as EnemyUnit;
        if (enemyUnit != null) {
            enemyUnit.Despawn();
        }
    }

    protected override void SubscribeToEvents() {
        base.SubscribeToEvents();
        Unit.OnCombatSoundHeard += OnSoundHeard;

        EnemyManager.Instance.OnUnitBroadcastMessage += OnUnitBroadcastMessage;
    }

    protected override void UnsubscribeToEvents() {
        base.UnsubscribeToEvents();
        Unit.OnCombatSoundHeard -= OnSoundHeard;

        EnemyManager.Instance.OnUnitBroadcastMessage -= OnUnitBroadcastMessage;
    }
    
    private void OnUnitBroadcastMessage(AIStateTransitionId transitionId, UnitController controller) {
        if(controller == this) {
            return;
        }
        TransitionState(transitionId);
    }

    #region LISTENERS

    protected override void OnTakeDamage(int damage, DamageType damageType, Unit attacker) {
        base.OnTakeDamage(damage, damageType, attacker);
        if(Health <= 0) {
            CurrentAIStateReadyToTransition(AIStateTransitionId.OnUnitDefeated);
            return;
        }
        if (attacker.UnitTags.HasFlag(UnitTags.Player) || attacker.UnitTags.HasFlag(UnitTags.Law_Enforcement)) {
            FocusedTarget = attacker;
        }
        CurrentAIStateReadyToTransition(AIStateTransitionId.OnUnitTakeDamage);
    }

    private void OnSoundHeard(IntVector3 origin, Unit source) {
        if (source != null && (source == Unit || source.UnitTags.HasFlag(Unit.UnitTags))) {
            return;
        }
        MapSpaceTarget = origin;
        CurrentAIStateReadyToTransition(AIStateTransitionId.OnCombatNoiseHeard);
    }

    #endregion
}
