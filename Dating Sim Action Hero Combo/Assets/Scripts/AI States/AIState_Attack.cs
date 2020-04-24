using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIState_Attack : AIState
{
    [SerializeField] protected NPCMoveController _moveController;

    protected Unit _target;
    protected UnitData _unitData;
    protected Weapon _equippedWeapon;

    public override void Enter(AIStateInitializationData initData = null) {
        _unitData = _controller.Data;
        _target = _controller.FocusedTarget;
        _equippedWeapon = _controller.EquippedWeapon;
        if (_moveController != null) {
            _moveController.ClearDestination();
            _moveController.SetLookTarget(_target.transform);
        }
        base.Enter(initData);
    }
    
    protected virtual bool CanAttack() {
        return true;
    }

    protected virtual void Attack() {
        int ammo = _equippedWeapon.Data.ClipSize;
        _equippedWeapon.Use(_equippedWeapon.Data.ActivateTime, _unit, ref ammo);
    }
}
