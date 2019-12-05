using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Hitbox : MonoBehaviour
{
    private int _damage;
    private DamageType _damageType;

    public event Action OnHitBoxTriggered;

    public void Initialize(int damage, DamageType damageType) {
        _damage = damage;
        _damageType = damageType;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        IDamageable damageable = collision.collider.GetComponent<IDamageable>();
        if(damageable != null) {
            damageable.TakeDamage(_damage, _damageType);
        }
        OnHitBoxTriggered?.Invoke();
    }
}
