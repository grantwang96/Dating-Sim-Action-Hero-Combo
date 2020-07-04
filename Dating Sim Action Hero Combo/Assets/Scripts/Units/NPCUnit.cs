using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class NPCUnit : Unit
{
    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [SerializeField] protected CircleCollider2D _collider;
    [SerializeField] protected NPCCombatController _combatController;

    public NPCCombatController CombatController => _combatController;

    public override void Initialize(string unitId, UnitData data) {
        _collider.radius = .5f; // replace with UnitData field
        MoveController.Initialize();
        base.Initialize(unitId, data);
    }
}
