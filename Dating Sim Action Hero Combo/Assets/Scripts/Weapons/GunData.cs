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

        // fire the shot
        FireShot(unit);
        // update weapon state
        state.LastActivateTime = Time.time;
    }

    protected virtual void FireShot(Unit unit) {
        // create bullet here
        PooledObject obj;
        if (!PooledObjectManager.Instance.UsePooledObject(_bulletPrefabId, out obj)) {
            CustomLogger.Log(name, $"{_bulletPrefabId} has not been registered yet. Registering now...");
            PooledObjectManager.Instance.RegisterPooledObject(_bulletPrefabId, 10);
            if(!PooledObjectManager.Instance.UsePooledObject(_bulletPrefabId, out obj)) {
                CustomLogger.Error($"{name} ({nameof(GunData)})", $"Could not retrieve pooled object for prefab id {obj}");
                return;
            }
        }
        Bullet bullet = obj as Bullet;
        if(bullet == null) {
            CustomLogger.Error($"{name} ({nameof(GunData)})", $"Retrieved pooled object was not of type {nameof(Bullet)}");
            return;
        }

        // calculate direction to fire and add force
        Vector2 velocity = unit.Front.up * _bulletSpeed;
        bullet.Setup(_power, _bulletLifeTime, unit.Front.position, velocity);
        bullet.Spawn();
    }
}
