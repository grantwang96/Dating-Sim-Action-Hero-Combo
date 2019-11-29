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
    [SerializeField] protected Transform _front;

    public override Transform Transform => transform;
    public override Transform Front => _front;
    public override IntVector3 MapPosition => LevelDataManager.Instance.WorldToArraySpace(transform.position);

    [SerializeField] protected NPCMoveController _moveController;
    public NPCMoveController MoveController => _moveController;

    public void Initialize(RuntimeAnimatorController animController, float size) {
        _animator.runtimeAnimatorController = animController;
        _collider.radius = size;
        _front.localPosition = Vector2.up * (size + 0.1f);
        _front.localEulerAngles = Vector2.up;

        MoveController.Initialize();
    }
}
