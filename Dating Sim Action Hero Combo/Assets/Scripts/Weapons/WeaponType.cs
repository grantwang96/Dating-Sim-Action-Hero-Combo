using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponType : ScriptableObject {

    [SerializeField] protected Bullet _bulletPrefab;

    [SerializeField] protected int _damage;
    [SerializeField] protected float _bulletLifeTime;

    [SerializeField] protected float _bulletForce;
    [SerializeField] protected Sprite _bulletSprite;

    [SerializeField] protected float _noiseRadius;
    public float noiseRadius { get { return _noiseRadius; } }
    [SerializeField] protected float _coolDownTime;

    [SerializeField] protected float _reloadTime;
    public float reloadTime { get { return _reloadTime; } }

    [SerializeField] protected int _clipCapacity;
    public int clipCapacity { get { return _clipCapacity; } }

    public virtual float Fire(Vector2 firingPoint, Vector2 firingDir, Transform source) {

        Bullet bullet = Instantiate(_bulletPrefab, firingPoint, Quaternion.Euler(0f, 0f, Mathf.Atan2(firingDir.y, firingDir.x)));
        bullet.srend.sprite = _bulletSprite;
        bullet.maxLifeTime = _bulletLifeTime;
        bullet.soundRadius = _noiseRadius;
        bullet.damage = _damage;
        bullet.owner = source;

        try {
            Rigidbody2D newBullet = bullet.GetComponent<Rigidbody2D>();
            newBullet.AddForce(firingDir * _bulletForce, ForceMode2D.Impulse);
        }
        catch(System.NullReferenceException nre) {
            Debug.Log(nre.Source);
        }

        return _coolDownTime;
    }
}
