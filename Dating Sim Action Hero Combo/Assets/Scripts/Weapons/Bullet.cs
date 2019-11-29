using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, PooledObject
{
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private TrailRenderer _trailRenderer;

    private int _power;
    private float _totalLifeTime;
    private bool _isLive;
    private Vector2 _initialVelocity;
    
    public void Setup(int power, float totalLifeTime, Vector2 newPosition, Vector2 velocity) {
        _power = power;
        _totalLifeTime = totalLifeTime;
        transform.position = newPosition;
        transform.up = velocity.normalized;
        _initialVelocity = velocity;
    }

    public void Spawn() {
        _trailRenderer.Clear();
        gameObject.SetActive(true);
        _rigidbody.isKinematic = false;
        _rigidbody.velocity = _initialVelocity;
        _isLive = true;
    }

    private void Update() {
        if (!_isLive) {
            return;
        }
        if(_totalLifeTime <= 0f) {
            Despawn();
        }
        _totalLifeTime -= Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        IDamageable damageable = collision.otherCollider.GetComponent<IDamageable>();
        if(damageable != null && _isLive) {
            damageable.TakeDamage(_power, DamageType.Normal);
        }
        Despawn();
    }

    public void Despawn() {
        _isLive = false;
        _rigidbody.isKinematic = false;
        gameObject.SetActive(false);
        PooledObjectManager.Instance.ReturnPooledObject(this.name, this);
    }
}
