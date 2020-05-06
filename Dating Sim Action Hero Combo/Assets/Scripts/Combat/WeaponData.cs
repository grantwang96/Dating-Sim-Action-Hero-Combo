using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponData : ScriptableObject
{
    [SerializeField] protected ActivateTime _weaponActivateTime;
    [SerializeField] private float _cooldownTime;
    public ActivateTime ActivateTime => _weaponActivateTime;
    [SerializeField] protected bool _isRanged;
    public bool IsRanged => _isRanged;

    [SerializeField] private int _clipSize;
    public int ClipSize => _clipSize;
    [SerializeField] private int _ammoPerUse;
    public int AmmoPerUse => _ammoPerUse;
    [SerializeField] private int _defaultStartAmount;
    public int DefaultStartAmount => _defaultStartAmount;
    [SerializeField] private float _reloadTime;
    public float ReloadTime => _reloadTime;

    [SerializeField] protected Sprite _sprite;
    [SerializeField] protected Sprite _icon;

    public Sprite Sprite => _sprite;
    public Sprite Icon => _icon;

    public virtual bool TryActivate(ActivateTime time, Unit unit, ActiveWeaponState state) {
        if((time & _weaponActivateTime) == 0) {
            return false;
        }
        // do weapon thing
        return PerformAction(unit, state);
    }

    protected abstract bool PerformAction(Unit unit, ActiveWeaponState state);

    protected bool HasWeaponCooledDown(float lastActivateTime) {
        return (Time.time - lastActivateTime) >= _cooldownTime;
    }
}

[System.Flags]
public enum ActivateTime {
    OnPress = (1 << 0),
    OnHeld = (1 << 1),
    OnRelease = (1 << 2)
}

public class ActiveWeaponState {
    public float LastActivateTime;

    public virtual void Clear() {
        LastActivateTime = Time.time;
    }
}
