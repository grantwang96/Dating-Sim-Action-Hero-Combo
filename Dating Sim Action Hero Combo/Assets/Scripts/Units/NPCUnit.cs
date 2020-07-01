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

    public event Action OnUnitInitialized; // when a controller has taken control of this unit GO

    public void Initialize(RuntimeAnimatorController animController, float size) {
        _animator.runtimeAnimatorController = animController;
        _collider.radius = size;
        OnUnitInitialized?.Invoke();

        MoveController.Initialize();
    }
}
