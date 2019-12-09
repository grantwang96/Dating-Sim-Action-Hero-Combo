using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon Data/Gun")]
public class GunData : WeaponData {
    
    [SerializeField] private string _bulletPrefabId;
    [SerializeField] private int _power;
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private float _bulletLifeTime;

    protected override void PerformAction(Unit unit, ActiveWeaponState state) {
        if (!HasWeaponCooledDown(state.LastActivateTime)) {
            // fail to fire
            return;
        }

        // start firing the shot
        InitiateShot(unit);
        // update weapon state
        state.LastActivateTime = Time.time;
    }

    protected virtual void InitiateShot(Unit unit) {

        // create bullet here
        PooledObject obj;
        if (!PooledObjectManager.Instance.UsePooledObject(_bulletPrefabId, out obj)) {
            // register bullets if not already there
            PooledObjectManager.Instance.RegisterPooledObject(_bulletPrefabId, 10);
            InitiateShot(unit);
            return;
        }
        Bullet bullet = obj as Bullet;
        if (bullet == null) {
            CustomLogger.Error($"{name} ({nameof(GunData)})", $"Retrieved pooled object was not of type {nameof(Bullet)}");
            return;
        }
        FireShot(unit, bullet);
    }

    protected virtual void FireShot(Unit unit, Bullet bullet) {
        // calculate direction to fire and add force
        Vector2 velocity = unit.Front.up * _bulletSpeed;
        bullet.Setup(_power, _bulletLifeTime, unit.Front.position, velocity);
        bullet.Spawn();
    }
}
