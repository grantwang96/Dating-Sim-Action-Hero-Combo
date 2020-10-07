using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_ScanForTarget : AIState
{
    [SerializeField] private AIState _onTargetFound;
    [SerializeField] private FieldOfViewVisualizer _visualizedFieldOfView;

    private DetectableTags _expectedDetectableTags;

    public override void Enter(AIStateInitializationData initData = null) {
        base.Enter(initData);
        _expectedDetectableTags = _unit.TargetManager.CurrentTarget.DetectableTags;
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
        Unit currentTarget = _unit.TargetManager.CurrentTarget;
        bool foundTarget = true;
        foundTarget &= (currentTarget.DetectableTags & _expectedDetectableTags) != 0;
        foundTarget &= _unit.TargetManager.CanSeeTarget(currentTarget);
        return foundTarget;
    }
} 
