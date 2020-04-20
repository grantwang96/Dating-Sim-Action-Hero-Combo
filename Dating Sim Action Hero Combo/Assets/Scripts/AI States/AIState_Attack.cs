using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIState_Attack : AIStateDataObject
{
    
}

public abstract class ActiveAttackState : ActiveAIState {

    protected Unit _unit;
    protected Unit _target;
    protected UnitData _unitData;
    protected NPCMoveController _moveController;
    protected Weapon _equippedWeapon;

    public ActiveAttackState(NPCUnitController controller) {
        _unit = controller.Unit;
        _unitData = controller.Data;
        _target = controller.FocusedTarget;
        _equippedWeapon = controller.EquippedWeapon;
        _moveController = _unit.GetComponent<NPCMoveController>();
        if (_moveController != null) {
            _moveController.ClearDestination();
            _moveController.SetLookTarget(_target.transform);
        }
    }

    protected virtual bool CanAttack() {
        return true;
    }

    protected virtual void Attack() {
        int ammo = _equippedWeapon.Data.ClipSize;
        _equippedWeapon.Use(_equippedWeapon.Data.ActivateTime, _unit, ref ammo);
    }
}
