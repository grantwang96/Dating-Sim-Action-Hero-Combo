using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class Unit : MonoBehaviour, IDamageable, ISoundListener
{
    [SerializeField] private UnitTags _unitTags;
    public UnitTags UnitTags => _unitTags;

    [SerializeField] protected Animator _animator;
    public Animator Animator => _animator;
    public abstract MoveController MoveController { get; }

    public abstract Transform Transform { get; }
    public abstract Transform Front { get; }

    public event Action<Unit, UnitTags> OnUnitTagsSet;
    public event Action<int, DamageType, Unit> OnTakeDamage;
    public event Action<int> OnHealDamage;

    public event Action<IntVector3, Unit> OnCombatSoundHeard;

    public virtual void SetUnitTags(UnitTags newTags) {
        OnUnitTagsSet?.Invoke(this, newTags);
        _unitTags = newTags;
    }

    public virtual void TakeDamage(int damage, DamageType damageType, Unit attacker) {
        OnTakeDamage?.Invoke(damage, damageType, attacker);
    }

    public virtual void Heal(int damage) {
        OnHealDamage?.Invoke(damage);
    }

    public virtual void OnSoundHeard(IntVector3 origin, Unit source = null) {
        OnCombatSoundHeard?.Invoke(origin, source);
    }

    public virtual void SetUnitComponent() {

    }
}

[System.Flags]
public enum UnitTags {
    Player = (1 << 0),
    Civilian = (1 << 1),
    Law_Enforcement = (1 << 2),
    Enemy = (1 << 3),
    Date = (1 << 4)
}
