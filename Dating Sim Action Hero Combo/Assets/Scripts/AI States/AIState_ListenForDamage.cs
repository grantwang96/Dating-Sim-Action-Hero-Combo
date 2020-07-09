using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_ListenForDamage : AIState
{
    [SerializeField] private NPCTargetManager _npcTargetManager;
    [SerializeField] private AIState _onTakeDamage;

    public override void Enter(AIStateInitializationData initData = null) {
        base.Enter(initData);
        _unit.Damageable.OnTakeDamage += OnTakeDamage;
    }

    public override void Exit(AIState nextState) {
        base.Exit(nextState);
        _unit.Damageable.OnTakeDamage -= OnTakeDamage;
    }

    private void OnTakeDamage(int damage, DamageType damageType, Unit attacker) {
        _npcTargetManager.OverrideCurrentTarget(attacker);
        SetReadyToTransition(_onTakeDamage);
    }
}
