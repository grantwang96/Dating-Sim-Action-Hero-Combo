using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_ScanForUnits : AIState
{
    [SerializeField] private AIState _onUnitFound;
    [SerializeField] private GameObject _visualzedFieldOfView;

    public override void Enter(AIStateInitializationData initData = null) {
        base.Enter(initData);
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

    public override void Exit(AIState nextState) {
        base.Exit(nextState);
        _visualzedFieldOfView.SetActive(false);
    }

    private void OnFoundHostile() {
        SetReadyToTransition(_onUnitFound);
    }

    private bool ScanForTarget() {
        return _unit.TargetManager.ScanForTarget(_unit.TargetManager.CurrentTarget);
    }
}
