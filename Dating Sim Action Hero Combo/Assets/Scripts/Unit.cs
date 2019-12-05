using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class Unit : MonoBehaviour, IDamageable
{
    [SerializeField] protected Animator _animator;

    public abstract Transform Transform { get; }
    public abstract Transform Front { get; }
    public abstract IntVector3 MapPosition { get; }

    public event Action<int, DamageType> OnTakeDamage;
    public event Action<int> OnHealDamage;

    public virtual void TakeDamage(int damage, DamageType damageType) {
        OnTakeDamage?.Invoke(damage, damageType);
    }

    public virtual void Heal(int damage) {
        OnHealDamage?.Invoke(damage);
    }
}
