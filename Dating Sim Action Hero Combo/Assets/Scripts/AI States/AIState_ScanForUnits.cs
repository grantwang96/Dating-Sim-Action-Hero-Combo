using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_ScanForUnits : AIState
{
    [SerializeField] private AIState _onUnitFound;
    [SerializeField] private GameObject _visualzedFieldOfView;
    
    protected override void OnEnter() {
        base.OnEnter();
        _visualzedFieldOfView.SetActive(true);
    }

    public override void Execute() {
        base.Execute();
        bool foundHostile = ScanForTarget();
        if (foundHostile) {
            OnFoundHostile();
            return;
        }
        return;
    }
    
    protected override void OnExit() {
        base.OnExit();
        _visualzedFieldOfView.SetActive(false);
    }

    private void OnFoundHostile() {
        SetReadyToTransition(_onUnitFound);
    }

    private bool ScanForTarget() {
        return _unit.TargetManager.CanSeeTarget(_unit.TargetManager.CurrentTarget);
    }
}
