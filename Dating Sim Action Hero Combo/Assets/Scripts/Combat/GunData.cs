using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon Data/Gun")]
public class GunData : WeaponData {

    private const string BulletAssetId = "Bullet";
    private const string WeaponSoundBoxId = "WeaponSoundBox";

    [SerializeField] private int _power;
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private float _bulletLifeTime;

    [SerializeField] private float _soundBoxSize;

    protected override bool PerformAction(Unit unit, ActiveWeaponState state) {
        if (!HasWeaponCooledDown(state.LastActivateTime)) {
            // fail to fire
            return false;
        }

        // start firing the shot
        InitiateShot(unit);
        // update weapon state
        state.LastActivateTime = Time.time;
        return true;
    }

    protected virtual void InitiateShot(Unit unit) {

        // create bullet here
        PooledObject obj;
        if (!PooledObjectManager.Instance.UsePooledObject(BulletAssetId, out obj)) {
            // register bullets if not already there
            PooledObjectManager.Instance.RegisterPooledObject(BulletAssetId, 10);
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
        Vector2 velocity = unit.MoveController.Front.up * _bulletSpeed;
        BulletInitializationData initData = new BulletInitializationData() {
            Power = _power,
            TotalLifeTime = _bulletLifeTime,
            Position = unit.MoveController.Front.position,
            Velocity = velocity,
            Owner = unit
        };
        bullet.Initialize(initData);
        bullet.Spawn();
        GenerateSoundBox(unit);
    }

    protected virtual void GenerateSoundBox(Unit unit) {
        PooledObject weaponSoundBoxPO;
        if (!PooledObjectManager.Instance.UsePooledObject(WeaponSoundBoxId, out weaponSoundBoxPO)) {
            CustomLogger.Error(this.name, $"Could not retrieve weapon sound box pooled object with id: {WeaponSoundBoxId}");
            return;
        }
        WeaponSoundBox soundBox = weaponSoundBoxPO as WeaponSoundBox;
        if(soundBox == null) {
            CustomLogger.Error(this.name, $"Object {weaponSoundBoxPO} was not of type {nameof(WeaponSoundBox)}");
            return;
        }
        WeaponSoundBoxInitializationData initData = new WeaponSoundBoxInitializationData() {
            TargetSize = _soundBoxSize,
            StartPosition = unit.MoveController.Front.position,
            Source = unit
        };
        soundBox.Initialize(initData);
        soundBox.Spawn();
    }
}
