using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_ScanForTarget : AIState
{
    [SerializeField] private AIState _onTargetFound;

    public override void Execute() {
        base.Execute();
        bool foundHostile = ScanForTarget();
        if (foundHostile) {
            OnFoundHostile();
            return;
        }
        return;
    }

    private void OnFoundHostile() {
        SetReadyToTransition(_onTargetFound);
    }

    private bool ScanForTarget() {
        return _unit.TargetManager.ScanForTarget(_unit.TargetManager.CurrentTarget);
    }
}
