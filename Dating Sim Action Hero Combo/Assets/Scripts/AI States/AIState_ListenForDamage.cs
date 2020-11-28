using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_ListenForDamage : AIState
{
    [SerializeField] private AIState _onTakeDamage;
    
    protected override void OnEnter() {
        base.OnEnter();
        _unit.Damageable.OnTakeDamage += OnTakeDamage;
    }
    
    protected override void OnExit() {
        base.OnExit();
        _unit.Damageable.OnTakeDamage -= OnTakeDamage;
    }

    private void OnTakeDamage(int damage, DamageType damageType, Unit attacker) {
        _unit.TargetManager.OverrideCurrentTarget(attacker);
        SetReadyToTransition(_onTakeDamage);
    }
}
