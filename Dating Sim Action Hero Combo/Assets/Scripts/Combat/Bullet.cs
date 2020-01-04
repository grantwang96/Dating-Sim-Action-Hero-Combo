using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, PooledObject
{
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private TrailRenderer _trailRenderer;
    [SerializeField] private Hitbox _hitBox;

    private int _power;
    private float _totalLifeTime;
    private bool _isLive;
    private Vector2 _initialVelocity;
    private Unit _owner;
    private IDamageable _ownerDamageable;

    private void Awake() {
        _hitBox.OnHitBoxTriggered += OnHitBoxTriggered;
    }

    private void OnDestroy() {
        _hitBox.OnHitBoxTriggered -= OnHitBoxTriggered;
    }

    public void Setup(int power, float totalLifeTime, Vector2 newPosition, Vector2 velocity, Unit owner) {
        _power = power;
        _totalLifeTime = totalLifeTime;
        transform.position = newPosition;
        transform.up = velocity.normalized;
        _initialVelocity = velocity;
        _owner = owner;
        _ownerDamageable = owner.GetComponent<IDamageable>();
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

    private void OnHitBoxTriggered(Collider2D collider) {
        IDamageable damageable = collider.GetComponent<IDamageable>();
        if (damageable != null && damageable != _ownerDamageable) {
            damageable.TakeDamage(_power, DamageType.Normal, _owner);
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
