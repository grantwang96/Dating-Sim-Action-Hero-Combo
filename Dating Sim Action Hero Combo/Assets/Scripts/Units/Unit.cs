using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class Unit : MonoBehaviour
{
    [SerializeField] private UnitTags _unitTags;
    public UnitTags UnitTags => _unitTags;

    [SerializeField] protected Animator _animator;
    [SerializeField] protected MoveController _moveController;
    [SerializeField] protected UnitDamageable _damageable;
    [SerializeField] protected UnitAnimationController _animationController;
    [SerializeField] protected UnitData _unitData;

    public Animator Animator => _animator;
    public MoveController MoveController => _moveController;
    public IDamageable Damageable => _damageable;
    public IAnimationController AnimationController => _animationController;
    public UnitData UnitData => _unitData;
}

[System.Flags]
public enum UnitTags {
    Player = (1 << 0),
    Civilian = (1 << 1),
    Law_Enforcement = (1 << 2),
    Enemy = (1 << 3),
    Date = (1 << 4)
}
