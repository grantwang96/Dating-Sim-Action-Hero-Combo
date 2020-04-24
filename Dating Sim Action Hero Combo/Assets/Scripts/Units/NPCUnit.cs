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
    [SerializeField] protected AIState _onUnitInitializedState;

    [SerializeField] protected NPCMoveController _moveController;
    public override MoveController MoveController => _moveController;

    public override Transform Transform => transform;
    public override Transform Front => _front;
    public AIState OnUnitInitializedState => _onUnitInitializedState;

    public event Action<NPCUnitController> OnUnitInitialized; // when a controller has taken control of this unit GO

    public void Initialize(NPCUnitController controller, RuntimeAnimatorController animController, float size) {
        _animator.runtimeAnimatorController = animController;
        _collider.radius = size;
        _front.localPosition = Vector2.up * (size + 0.25f);
        _front.localEulerAngles = Vector2.up;
        OnUnitInitialized?.Invoke(controller);

        MoveController.Initialize();
    }
}
