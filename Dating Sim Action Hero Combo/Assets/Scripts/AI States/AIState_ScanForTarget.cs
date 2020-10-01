using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_ScanForTarget : AIState
{
    [SerializeField] private AIState _onTargetFound;
    [SerializeField] private FieldOfViewVisualizer _visualizedFieldOfView;

    public override void Enter(AIStateInitializationData initData = null) {
        base.Enter(initData);
        _visualizedFieldOfView.SetActive(true);
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
        _visualizedFieldOfView.SetActive(false);
    }

    private void OnFoundHostile() {
        SetReadyToTransition(_onTargetFound);
    }

    private bool ScanForTarget() {
        return _unit.TargetManager.ScanForTarget(_unit.TargetManager.CurrentTarget);
    }
}
