using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class Unit : MonoBehaviour, IDamageable
{
    public virtual int Health { get; protected set; }
    public virtual bool IsDead { get; protected set; }

    [SerializeField] protected Rigidbody2D _rigidbody;
    [SerializeField] protected Animator _animator;

    public abstract Transform Transform { get; }
    public abstract Transform Front { get; }
    public abstract IntVector3 MapPosition { get; }

    public virtual void TakeDamage(int damage, DamageType damageType) {
        if (IsDead) {
            return;
        }
        Health -= damage;
        if(Health <= 0) {
            IsDead = true;
        }
    }
}
