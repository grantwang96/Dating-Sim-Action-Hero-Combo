using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_QuckScanForTarget : AIState
{
    [SerializeField] private AIState _onTargetFound;
    [SerializeField] private AIState _onTargetLost;

    private DetectableTags _expectedDetectableTags;

    protected override void OnEnter() {
        base.OnEnter();
        if(_unit.TargetManager.CurrentTarget == null) {
            SetReadyToTransition(_onTargetLost);
            return;
        }
        _expectedDetectableTags = _unit.TargetManager.CurrentTarget.DetectableTags;
        bool foundTarget = ScanForTarget();
        if (foundTarget) {
            SetReadyToTransition(_onTargetFound);
        } else {
            SetReadyToTransition(_onTargetLost);
        }
    }

    private bool ScanForTarget() {
        Unit currentTarget = _unit.TargetManager.CurrentTarget;
        if (currentTarget == null) {
            SetReadyToTransition(_onTargetLost);
            return false;
        }
        bool foundTarget = true;
        foundTarget &= (currentTarget.DetectableTags & _expectedDetectableTags) != 0;
        foundTarget &= _unit.TargetManager.CanSeeTarget(currentTarget);
        return foundTarget;
    }
}
