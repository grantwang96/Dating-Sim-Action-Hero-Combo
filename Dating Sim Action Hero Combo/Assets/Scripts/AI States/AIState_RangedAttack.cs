using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI State/Ranged Attack")]
public class AIState_RangedAttack : AIStateDataObject
{
    protected override ActiveAIState GenerateActiveAIState(IUnitController controller) {
        ActiveRangedAttackState rangedAttackState = new ActiveRangedAttackState(controller);
        return rangedAttackState;
    }
}

public class ActiveRangedAttackState : ActiveAIState {

    private Unit _unit;
    private Unit _target;
    private UnitData _unitData;
    private NPCMoveController _moveController;
    private WeaponSlot _equippedWeapon;

    public ActiveRangedAttackState(IUnitController controller) {
        _unit = controller.Unit;
        _unitData = controller.Data;
        _target = controller.FocusedTarget;
        _equippedWeapon = controller.EquippedWeapon;
        _moveController = _unit.GetComponent<NPCMoveController>();
        if(_moveController != null) {
            _moveController.ClearDestination();
            _moveController.SetLookTarget(_target.transform);
        }
    }

    // attempt to fire weapon continuously
    public override bool OnExecute() {
        // fail if there is no move controller or target
        if(_moveController == null || _target == null) {
            SetNextTransition(AIStateTransitionId.OnUnitEnemyLost);
            return true;
        }
        base.OnExecute();
        bool canSeeTarget = ActiveScanState.Scan(_target, _unit.transform, _unitData.VisionRange, _unitData.VisionLayers);
        if (!canSeeTarget) {
            LostTarget();
            return true;
        }
        ActivateTime activateTime = _equippedWeapon.Data.ActivateTime;
        int totalAmmo = _equippedWeapon.Data.ClipSize;
        _equippedWeapon.Use(activateTime, _unit, ref totalAmmo); // AI ammo is a filthy lie
        return false;
    }

    private void LostTarget() {
        _moveController.SetLookTarget(null);
        SetNextTransition(AIStateTransitionId.OnUnitEnemyLost);
    }
}
