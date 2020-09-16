using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public abstract class NPCUnit : Unit
{
    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [SerializeField] protected CircleCollider2D _collider;
    [SerializeField] protected NPCTargetManager _targetManager;
    [SerializeField] protected NPCNavigator _navigator;
    [SerializeField] protected NPCCombatController _combatController;

    public NPCTargetManager TargetManager => _targetManager;
    public NPCNavigator Navigator => _navigator;
    public NPCCombatController CombatController => _combatController;

    public event Action<NPCUnit, UnitMessage> OnUnitMessageSent;
    public event Action<NPCUnit, UnitMessage> OnUnitMessageReceived;

    public override void Initialize(string unitId, UnitData data) {
        _collider.radius = .5f; // replace with UnitData field
        base.Initialize(unitId, data);
    }

    public void SendMessage(UnitMessage message) {
        OnUnitMessageSent?.Invoke(this, message);
    }

    protected override void SubscribeToEvents() {
        base.SubscribeToEvents();
        SubscribeToAllianceManager();
    }

    protected abstract void SubscribeToAllianceManager();
    protected abstract void UnsubscribeToAllianceManager();

    protected virtual void OnAllianceMessageSent(NPCUnit unit, UnitMessage message) {
        OnUnitMessageReceived?.Invoke(unit, message);
    }
}
